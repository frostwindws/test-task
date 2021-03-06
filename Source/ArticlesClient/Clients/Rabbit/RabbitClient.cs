﻿using System;
using ArticlesClient.Clients.Rabbit.Converters;
using ArticlesClient.Models;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ArticlesClient.Commands;
using ArticlesClient.Connected_Services.ArticlesService;
using RabbitMQ.Client.Exceptions;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Клиент обращения к Rabbit MQ.
    /// </summary>
    internal class RabbitClient : IDataClient
    {
        private readonly RabbitRequestProvider provider;
        private readonly AnnounceCommandsInvoker invoker;
        private readonly IMessageBodyConverter bodyConverter;
        private readonly string announceExchange;
        private ViewDataContainer subscribedViewData;

        /// <summary>
        /// Репозиторий статей.
        /// </summary>
        public IRepository<ArticleView> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев.
        /// </summary>
        public IRepository<CommentView> Comments { get; set; }

        /// <summary>
        /// Конструктор клиента обращения к Rabbit MQ.
        /// </summary>
        /// <param name="provider">Провайдер обращения к Rabbit.</param>
        /// <param name="invoker">Исполнитель команд для обработки сообщений анонсов.</param>
        /// <param name="bodyConverter">Конвертер тела сообщения.</param>
        /// <param name="requestQueue">Очередь для отправки сообщений.</param>
        /// <param name="announceExchange">Обмен для получения оповещений.</param>
        public RabbitClient(RabbitRequestProvider provider,
            AnnounceCommandsInvoker invoker,
            IMessageBodyConverter bodyConverter,
            string requestQueue,
            string announceExchange)
        {
            this.provider = provider;
            this.invoker = invoker;
            this.bodyConverter = bodyConverter;
            this.announceExchange = announceExchange;
            Articles = new RabbitArticlesRepository(provider, requestQueue, bodyConverter);
            Comments = new RabbitCommentsRepository(provider, requestQueue, bodyConverter);
        }

        /// <summary>
        /// Подписка на оповещения.
        /// </summary>
        /// <param name="viewData">Контейнер данных отображения, для подписки на оповещения.</param>
        public void SubscribeToAnnounce(ViewDataContainer viewData)
        {
            subscribedViewData = viewData;
            provider.SubscribeToAnnounce(announceExchange, ReceiveAnnounceData);
        }

        /// <summary>
        /// Обработка полученного оповещения.
        /// </summary>
        /// <param name="name">Имя типа оповещения.</param>
        /// <param name="message">Тело сообщения оповещения.</param>
        private void ReceiveAnnounceData(string name, byte[] message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (invoker.IsArticleCommand(name))
                    {
                        var article = bodyConverter.FromBody<RabbitResult<ArticleDto>>(message);
                        invoker.ExecuteForArticle(name, subscribedViewData, article.Data);
                    }
                    else if (invoker.IsCommentCommand(name))
                    {
                        var comment = bodyConverter.FromBody< RabbitResult<CommentDto>>(message);
                        invoker.ExecuteForComment(name, subscribedViewData, comment.Data);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            provider?.Dispose();
        }
    }
}
