using System;
using ArticlesWeb.Clients.Wcf;
using ArticlesWeb.Models;

namespace ArticlesWeb.Clients
{
    /// <summary>
    /// Интерфейс клиента получения данных для отображения.
    /// </summary>
    public interface IReadContext : IDisposable
    {
        /// <summary>
        /// Репозиторий статей.
        /// </summary>
        IRepository<Article> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев.
        /// </summary>
        IRepository<Comment> Comments { get; set; }
    }
}
