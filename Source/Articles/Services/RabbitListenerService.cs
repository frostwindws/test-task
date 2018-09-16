using Articles.Messaging;
using Articles.Messaging.Converters;
using Articles.Models;
using Articles.Services.Executors;
using Articles.Services.Models;
using AutoMapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Articles.Services
{
    /// <summary>
    /// Сервис прослушивания сообщений.
    /// </summary>
    public class RabbitListenerService : IListenerService
    {
        private readonly IRequestListener listener;
        private readonly DataContextFactory contextFactory;
        private readonly IRequestCommandInvoker commandInvoker;
        private readonly IMessageBodyConverter messageBodyConvertor;
        private readonly IModelValidator<Article> articlesValidator;
        private readonly IModelValidator<Comment> commentsValidator;

        /// <summary>
        /// Конструктор сервиса прослушивания сообщений.
        /// </summary>
        /// <param name="listener">Прослушиватель сообщений.</param>
        /// <param name="contextFactory">Фабрика контекстов.</param>
        /// <param name="commandInvoker">Исполнитель команд.</param>
        /// <param name="messageBodyConvertor">Исполнитель команд.</param>
        /// <param name="articlesValidator">Валидатор статьи.</param>
        /// <param name="commentsValidator">Валидатор комментария.</param>
        public RabbitListenerService(IRequestListener listener,
            DataContextFactory contextFactory,
            IRequestCommandInvoker commandInvoker,
            IMessageBodyConverter messageBodyConvertor,
            IModelValidator<Article> articlesValidator,
            IModelValidator<Comment> commentsValidator)
        {
            this.listener = listener;
            this.contextFactory = contextFactory;
            this.commandInvoker = commandInvoker;
            this.messageBodyConvertor = messageBodyConvertor;
            this.articlesValidator = articlesValidator;
            this.commentsValidator = commentsValidator;
        }

        /// <summary>
        /// Запуск прослушивания сообщений.
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены действия.</param>
        /// <returns></returns>
        public async Task StartListenning(CancellationToken cancellationToken)
        {
            await listener.Listen(ListenerOnAcceptMessage, cancellationToken);
        }

        /// <summary>
        /// Обработчик события получения комманды.
        /// </summary>
        /// <param name="message">Полученное сообщение.</param>
        private void ListenerOnAcceptMessage(Message message)
        {
            object result;
            var outMessage = new Message { Id = message.Id, Type = message.Type };
            try
            {
                using (IDataContext context = contextFactory.GetContext())
                {
                    if (commandInvoker.IsArticleCommand(message.Type))
                    {
                        // Запрос на обновление данных статей.
                        ArticleDto article = messageBodyConvertor.FromBody<ArticleDto>(message.Body);
                        Article dbArticle = Mapper.Map<Article>(article);
                        dbArticle = commandInvoker.ExecuteForArticle(message.Type, context, dbArticle, articlesValidator);
                        result = new ResultDto<ArticleDto> { Success = true, Data = Mapper.Map<ArticleDto>(dbArticle) };
                    }
                    else if (commandInvoker.IsCommentCommand(message.Type))
                    {
                        // Запрос на обновление данных комментариев.
                        CommentDto comment = messageBodyConvertor.FromBody<CommentDto>(message.Body);
                        Comment dbComment = Mapper.Map<Comment>(comment);
                        dbComment = commandInvoker.ExecuteForComment(message.Type, context, dbComment, commentsValidator);
                        result = new ResultDto<CommentDto> { Success = true, Data = Mapper.Map<CommentDto>(dbComment) };
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Received unknown command type message \"{message.Type}\"");
                    }
                }

                outMessage.Body = messageBodyConvertor.ToBody(result);

                // Оповещение всех клиентов о произведенном действии
                listener.Announce(outMessage); 
            }
            catch (Exception e)
            {
                // В случае ошибки обработки запроса ответ отправляется клиенту напрямую
                Log.Error(e, "Error accured while processing request {Id} with type {Type}: {Message}", message.Id, message.Type, e.GetBaseException().Message);
                result = new ResultDto<object>
                {
                    Success = false,
                    Message = $"Error occured while processing your request: {e.Message}",
                    Data = null
                };

                outMessage.Body = messageBodyConvertor.ToBody(result);
            }

            // Оповещение клиента, сформировавшего запрос о результате выполнения
            listener.Reply(message.ReplyTo, outMessage);
        }
    }
}