using System.Data;
using Articles.Models;

namespace ArticlesService.Dapper
{
    /// <summary>
    /// Контекст работы со статьями и комментариями к ним через Dapper.
    /// </summary>
    public class DapperContext : IDataContext
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
            Articles = new DapperArticlesRepository(connection);
            Comments = new DapperCommentsRepository(connection);
        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public void Commit()
        {
        }

        /// <summary>
        /// Отмена изменений
        /// </summary>
        public void Rollback()
        {
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