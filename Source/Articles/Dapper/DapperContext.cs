using System;
using System.Data;
using Articles.Models;

namespace Articles.Dapper
{
    /// <summary>
    /// Контекст работы со статьями и комментариями к ним через Dapper.
    /// </summary>
    public class DapperContext : IDataContext, IDisposable
    {
        private readonly IDbConnection connection;

        /// <inheritdoc />
        public IArticlesRepository Articles { get; }

        /// <inheritdoc />
        public ICommentsRepository Comments { get; }

        /// <summary>
        /// Конструктор контекста.
        /// </summary>
        /// <param name="connection"></param>
        public DapperContext(IDbConnection connection)
        {
            this.connection = connection;
            Articles = new ArticlesRepository(connection);
            Comments = new CommentsRepository(connection);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}