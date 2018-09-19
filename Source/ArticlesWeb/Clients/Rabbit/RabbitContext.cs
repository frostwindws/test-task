using System;
using System.Threading.Tasks;
using ArticlesService;
using ArticlesWeb.Clients.Rabbit.Commands;
using ArticlesWeb.Clients.Rabbit.Converters;
using ArticlesWeb.Commands;
using ArticlesWeb.Hubs;
using ArticlesWeb.Models;
using AutoMapper;

namespace ArticlesWeb.Clients.Rabbit
{
    /// <summary>
    /// Контекст обновления через RabbitMQ
    /// </summary>
    public class RabbitContext : IUpdateContext
    {
        private readonly RabbitRequestProvider provider;
        private readonly AnnounceCommandsInvoker invoker;
        private readonly IMessageBodyConverter bodyConverter;
        private readonly string requestQueue;
        private readonly string announceExchange;
        private UpdatesHub hubContext;

        /// <summary>
        /// Конструктор контекста обновления через RabbitMQ
        /// </summary>
        /// <param name="provider">Провайдер обращения к Rabbit.</param>
        /// <param name="invoker">Исполнитель команд для обработки сообщений анонсов.</param>
        /// <param name="bodyConverter">Конвертер тела сообщения.</param>
        /// <param name="requestQueue">Очередь для отправки сообщений.</param>
        /// <param name="announceExchange">Обмен для получения оповещений.</param>
        public RabbitContext(RabbitRequestProvider provider,
            AnnounceCommandsInvoker invoker,
            IMessageBodyConverter bodyConverter,
            string requestQueue,
            string announceExchange)
        {
            this.provider = provider;
            this.invoker = invoker;
            this.bodyConverter = bodyConverter;
            this.requestQueue = requestQueue;
            this.announceExchange = announceExchange;
        }

        /// <summary>
        /// Отправка запроса на обновление для статьи.
        /// </summary>
        /// <param name="name">Имя команды обновления.</param>
        /// <param name="article">Обновляемая статья.</param>
        public void SendUpdateForArticle(string name, Article article)
        {
            byte[] message = bodyConverter.ToBody(Mapper.Map<ArticleDto>(article));
            provider.SendRequest(requestQueue, name, message);
        }

        /// <summary>
        /// Отправка запроса на обновление комментария.
        /// </summary>
        /// <param name="name">Имя команды обновления.</param>
        /// <param name="comment">Обновляемый комментарий.</param>
        public void SendUpdateForComment(string name, Comment comment)
        {
            byte[] message = bodyConverter.ToBody(Mapper.Map<CommentDto>(comment));
            provider.SendRequest(requestQueue, name, message);
        }

        /// <summary>
        /// Подписка на события обновления.
        /// </summary>
        public void SubscribeToUpdates(UpdatesHub hubContext)
        {
            this.hubContext = hubContext;
            provider.SubscribeToAnnounce(announceExchange, OnReceived);
        }

        /// <summary>
        /// Метод обработки событий обновления.
        /// </summary>
        /// <param name="type">Имя произошедшей команды обновления.</param>
        /// <param name="body">Тело сообщения обновления.</param>
        private async Task OnReceived(string type, byte[] body)
        {
            // Тело сообщения интерпретируется в звисимости от типа сообщения
            object data = null;
            if (invoker.IsArticleCommand(type))
            {   
                var result = bodyConverter.FromBody<RabbitResult<ArticleDto>>(body);
                data = Mapper.Map<Article>(result.Data);
            }
            else if (invoker.IsArticleCommand(type))
            {
                var result = bodyConverter.FromBody<RabbitResult<CommentDto>>(body);
                data = Mapper.Map<Comment>(result.Data);
            }

            if (data != null)
            {
                //https://stackoverflow.com/questions/37318209/asp-net-core-rc2-signalr-hub-context-outside-request-thread
                await hubContext.SendUpdate(type, data);
            }
        }
    }
}
