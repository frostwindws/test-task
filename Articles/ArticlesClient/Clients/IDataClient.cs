using System;
using ArticlesClient.Models;

namespace ArticlesClient.Clients
{
    /// <summary>
    /// Интерфейс клиента получения данных для отображения
    /// </summary>
    public interface IDataClient : IDisposable
    {
        /// <summary>
        /// Репозиторий статей
        /// </summary>
        IRepository<ArticleView> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев
        /// </summary>
        IRepository<CommentView> Comments { get; set; }
    }
}
