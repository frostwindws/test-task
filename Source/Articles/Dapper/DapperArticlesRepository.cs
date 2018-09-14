using System.Collections.Generic;
using System.Data;
using Articles.Models;
using Dapper;
using Serilog;

namespace Articles.Dapper
{
    /// <summary>
    /// Репозиторий статей.
    /// </summary>
    public class DapperArticlesRepository : IArticlesRepository
    {
        // Для сокращения объема запрашиваемых данных текст передается только при запросе отдельной статьи
        private const string GetQuery = "select id, title, author, created from articles"; 
        private const string FindQuery = "select id, title, author, content, created from articles where id = @id";
        private const string CreateQuery = "insert into articles(title, author, content) values(@Title, @Author, @Content) returning id, title, author, content, created";
        private const string UpdateQuery = "update articles set title = @Title, content = @Content where id = @Id returning id, title, author, content, created";
        private const string DeleteQuery = "delete from articles where id = @id";
        private const string ExistsQuery = "select exists(select id from articles a where a.\"{0}\" = @value)";

        private readonly IDbConnection connection;

        /// <summary>
        /// Конструктор репозитория статей.
        /// </summary>
        public DapperArticlesRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Получить список статей.
        /// </summary>
        /// <returns>Возвращает полный список статей.</returns>
        public IEnumerable<Article> GetCollection()
        {
            return connection.Query<Article>(GetQuery);
        }

        /// <summary>
        /// Проверка наличия статьи с указанным свойством.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <returns>True, если статья с указанным свойством уже существует.</returns>
        public bool Exists(string propertyName, string value)
        {
            return connection.QueryFirst<bool>(string.Format(ExistsQuery, propertyName.ToLowerInvariant()), new { value });
        }

        /// <summary>
        /// Найти статью по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи.</param>
        /// <returns>
        /// Возвращает найденую статью.
        /// Если статья отсутствует - возвращает null.</returns>
        public Article Get(long id)
        {
            return connection.QuerySingleOrDefault<Article>(FindQuery, new { id });
        }

        /// <summary>
        /// Создать новую статью.
        /// </summary>
        /// <param name="record">Создаваемая статья.</param>
        /// <returns>Возвращает идентификатор созданной статьи.</returns>
        public Article Create(Article record)
        {
            var createdArticle = connection.QuerySingle<Article>(CreateQuery, record);
            Log.Information($"Created new article (Id={createdArticle.Id}): \"{createdArticle.Title}\" by {createdArticle.Author}");
            return createdArticle;
        }

        /// <summary>
        /// Обновление имеющейся статьи.
        /// </summary>
        /// <param name="record">Обновляемая статья.</param>
        public Article Update(Article record)
        {
            var updatedArticle = connection.QuerySingle<Article>(UpdateQuery, record);
            Log.Information($"Article updated (Id={updatedArticle.Id}): \"{updatedArticle.Title}\"");
            return updatedArticle;
        }

        /// <summary>
        /// Удалить статью.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи.</param>
        public void Delete(long id)
        {
            connection.Execute(DeleteQuery, new { id });
            Log.Information($"Article deleted (Id={id})");
        }
    }
}