using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Dapper;
using Npgsql;
using Serilog;

namespace Articles.Models
{
    /// <summary>
    /// Репозиторий статей
    /// </summary>
    public class ArticlesRepository : IArticlesRepository
    {
        // Для сокращения объема запрашиваемых данных текст передается только при запросе отдельной статьи
        private const string GetQuery = "select id, title, author, content, created from articles"; 
        private const string FindQuery = "select * from articles where id = @id";
        private const string CreateQuery = "insert into articles(title, author, content) values(@Title, @Author, @Content) returning id";
        private const string UpdateQuery = "update articles set title = @Title, author = @Author, content = @Content where id = @Id";
        private const string DeleteQuery = "delete from articles where id = @id";

        private readonly IDbConnection connection;

        /// <summary>
        /// Конструктор репозитория статей.
        /// </summary>
        public ArticlesRepository()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            connection = new NpgsqlConnection(connectionString);
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
        /// Найти статью по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи</param>
        /// <returns>
        /// Возвращает найденую статью.
        /// Если статья отсутствует - возвращает null</returns>
        public Article Find(long id)
        {
            return connection.QuerySingleOrDefault<Article>(FindQuery, new { id });
        }

        /// <summary>
        /// Создать новую статью
        /// </summary>
        /// <param name="record">Создаваемая статья</param>
        /// <returns>Возвращает идентификатор созданной статьи</returns>
        public long Create(Article record)
        {
            var id = connection.QuerySingle<long>(CreateQuery, record);
            Log.Information($"Created new article (Id={id}): \"{record.Title}\" by {record.Author}");
            return id;
        }

        /// <summary>
        /// Обновление имеющейся статьи
        /// </summary>
        /// <param name="record">Обновляемая статья</param>
        public void Update(Article record)
        {
            connection.Execute(UpdateQuery, record);
            Log.Information($"Article updated (Id={record.Id}): \"{record.Title}\" by {record.Author}");
        }

        /// <summary>
        /// Удалить статью
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи</param>
        public void Delete(long id)
        {
            connection.Execute(DeleteQuery, new { id });
            Log.Information($"Article deleted (Id={id})");
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}