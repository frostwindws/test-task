using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Dapper;
using Npgsql;
using Serilog;

namespace Articles.Models
{
    /// <summary>
    /// Репозиторий коментариев
    /// </summary>
    public class CommentsRepository : ICommentsRepository
    {
        private const string GetQuery = "select * from comments";
        private const string GetForArticleQuery = GetQuery + " where articleid = @articleId";
        private const string FindQuery = GetQuery + " where id = @id";
        private const string CreateQuery = "insert into comments(articleid, author, content) values(@ArticleId, @Author, @Content) returning id";
        private const string UpdateQuery = "update comments set author = @Author, content = @Content where id = @Id";
        private const string DeleteQuery = "delete from comments where id = @id";

        private readonly IDbConnection connection;

        /// <summary>
        /// Конструктор репозитория коментариев.
        /// </summary>
        public CommentsRepository()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            connection = new NpgsqlConnection(connectionString);
        }
        public IEnumerable<Comment> GetCollection()
        {
            return connection.Query<Comment>(GetQuery);
        }

        /// <summary>
        /// Получить список комментариев для статьи
        /// </summary>
        /// <param name="articleId">Идентификатор статьи</param>
        /// <returns>Возвращает список комментариев для указанной статьи</returns>
        public IEnumerable<Comment> GetForArticle(long articleId)
        {
            return connection.Query<Comment>(GetForArticleQuery, new { articleId });
        }


        /// <summary>
        /// Найти комментарий по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария.</param>
        /// <returns>
        /// Возвращает найденный комментарий.
        /// Если комментарий с указанным идентификатором отсутствует, возвращает null.
        /// </returns>
        public Comment Find(long id)
        {
            return connection.QuerySingleOrDefault<Comment>(FindQuery, new { id });
        }

        /// <summary>
        /// Создать новый комментарий
        /// </summary>
        /// <param name="comment">Создаваемый комментарий</param>
        /// <returns>Возвращает идентификатор созданного комментария</returns>
        public long Create(Comment comment)
        {
            long id = connection.QuerySingle<long>(CreateQuery, comment);
            Log.Information($"Created new comment (Id={id}) by {comment.Author} for article id = {comment.ArticleId}");
            return id;
        }

        /// <summary>
        /// Обновить имеющийся комментарий
        /// </summary>
        /// <param name="comment">Обновляемый комментарий</param>
        public void Update(Comment comment)
        {
            connection.Execute(UpdateQuery, comment);
            Log.Information($"Comment updated (Id={comment.Id}) by {comment.Author} for article id = {comment.ArticleId}");
        }

        /// <summary>
        /// Удалить комментарий
        /// </summary>
        /// <param name="id">Идентификатор комментария</param>
        public void Delete(long id)
        {
            connection.Execute(DeleteQuery, new { id });
            Log.Information($"Comment deleted (Id={id})");
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