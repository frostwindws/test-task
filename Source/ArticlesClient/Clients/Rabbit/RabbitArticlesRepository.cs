using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ArticlesClient.Clients.Rabbit.Converters;
using ArticlesClient.Commands;
using ArticlesClient.Connected_Services.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Rabbit MQ репозиторий статей.
    /// </summary>
    internal class RabbitArticlesRepository : IRepository<ArticleView>
    {
        private readonly RabbitRequestProvider provider;
        private readonly string requestQueue;
        private readonly IMessageBodyConverter bodyConverter;

        /// <summary>
        /// Конструктор репозитория статей.
        /// </summary>
        /// <param name="provider">Используемый провайдер для обращения к RabbitMQ.</param>
        /// <param name="requestQueue">Очередь для отправки сообщений.</param>
        /// <param name="bodyConverter">Ковертер тел сообщения.</param>
        public RabbitArticlesRepository(RabbitRequestProvider provider, string requestQueue, IMessageBodyConverter bodyConverter)
        {
            this.provider = provider;
            this.requestQueue = requestQueue;
            this.bodyConverter = bodyConverter;
        }

        /// <summary>
        /// Получить все статьи
        /// Метод не используется.
        /// </summary>
        /// <returns>Перечисление всех статей.</returns>
        public Task<IEnumerable<ArticleView>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить статью по ее идентификатору
        /// Метод не используется.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи.</param>
        /// <returns>Найденная статья, либо null  в случае отсутствия статьи с указанным идентификатором.</returns>
        public Task<ArticleView> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Добавление новой статьи.
        /// </summary>
        /// <param name="record">Добавляемая статья.</param>
        /// <returns>Данные созданной статьи.</returns>
        public async Task<ArticleView> AddAsync(ArticleView record)
        {
            byte[] message = bodyConverter.ToBody(Mapper.Map<ArticleDto>(record));
            byte[] response = await provider.SendRequest(requestQueue, CommandNames.CreateArticle, message);
            var result = bodyConverter.FromBody<RabbitResult<ArticleDto>>(response);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<ArticleView>(result.Data);
        }

        /// <summary>
        /// Обновление существующей статьи.
        /// </summary>
        /// <param name="record">Данные статьи для ее обновления.</param>
        /// <returns>Обновленная статья.</returns>
        public async Task<ArticleView> UpdateAsync(ArticleView record)
        {
            byte[] message = bodyConverter.ToBody(Mapper.Map<ArticleDto>(record));
            byte[] response = await provider.SendRequest(requestQueue, CommandNames.UpdateArticle, message);
            var result = bodyConverter.FromBody<RabbitResult<ArticleDto>>(response);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<ArticleView>(result.Data);
        }

        /// <summary>
        /// Удаление существующей статьи.
        /// </summary>
        /// <param name="record">Удаляемая статья.</param>
        public async Task DeleteAsync(ArticleView record)
        {
            byte[] message = bodyConverter.ToBody(Mapper.Map<ArticleDto>(record));
            byte[] response = await provider.SendRequest(requestQueue, CommandNames.DeleteArticle, message);
            var result = bodyConverter.FromBody<RabbitResult<ArticleDto>>(response);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }
        }
    }
}
