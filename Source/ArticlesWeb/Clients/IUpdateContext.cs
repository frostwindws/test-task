using System;
using ArticlesWeb.Hubs;
using ArticlesWeb.Models;
using Microsoft.AspNetCore.SignalR;

namespace ArticlesWeb.Clients
{
    /// <summary>
    /// Интерфейс контекста обновления данных.
    /// </summary>
    public interface IUpdateContext
    {
        /// <summary>
        /// Подписка на события обновления.
        /// </summary>
        /// <param name="hubContext">Контекст хаба для оповещения</param>
        void SubscribeToUpdates(UpdatesHub hubContext);

        /// <summary>
        /// Отправка запроса на обновление для статьи.
        /// </summary>
        /// <param name="name">Имя команды обновления.</param>
        /// <param name="article">Обновляемая статья.</param>
        void SendUpdateForArticle(string name, Article article);

        /// <summary>
        /// Отправка запроса на обновление комментария.
        /// </summary>
        /// <param name="name">Имя команды обновления.</param>
        /// <param name="comment">Обновляемый комментарий.</param>
        void SendUpdateForComment(string name, Comment comment);
    }
}
