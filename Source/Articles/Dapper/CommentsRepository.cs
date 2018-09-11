using System.Collections.Generic;
using System.Data;
using Articles.Models;
using Dapper;
using Serilog;

namespace Articles.Dapper
{
    /// <summary>
    /// Репозиторий коментариев
    /// </summary>
    public class CommentsRepository : ICommentsRepository
    {
        private const string GetQuery = "select id, articleid, author, content, created  from comments";
        private const string GetForArticleQuery = GetQuery + " where articleid = @articleId";
        private const string FindQuery = GetQuery + " where id = @id";
        private const string CreateQuery = "insert into comments(articleid, author, content) values(@ArticleId, @Author, @Content) returning id, articleid, author, content, created";
        private const string UpdateQuery = "update comments set content = @Content where id = @Id returning id, articleid, author, content, created"; // У комментариев может обновляться только контент
        private const string DeleteQuery = "delete from comments where id = @id";
        private const string ExistsQuery = "select exists(select id from comments a where a.\"{0}\" = @value)";

        private readonly IDbConnection connection;

        /// <summary>
        /// Конструктор репозитория коментариев.
        /// </summary>
        public CommentsRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Получить все имеющиеся комментарии
        /// </summary>
        /// <returns></returns>
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
        public Comment Get(long id)
        {
            return connection.QuerySingleOrDefault<Comment>(FindQuery, new { id });
        }

        /// <summary>
        /// Проверка наличия комментария с указанным свойством
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value">Значение свойства</param>
        /// <returns>True, если статья с указанным свойством уже существует</returns>
        public bool Exists(string propertyName, string value)
        {
            return connection.QueryFirst<bool>(string.Format(ExistsQuery, propertyName.ToLowerInvariant()), new { value });
        }

        /// <summary>
        /// Создать новый комментарий
        /// </summary>
        /// <param name="comment">Создаваемый комментарий</param>
        /// <returns>Возвращает идентификатор созданного комментария</returns>
        public Comment Create(Comment comment)
        {
            Comment createdComment = connection.QuerySingle<Comment>(CreateQuery, comment);
            Log.Information($"Created new comment (Id={createdComment.Id}) by {createdComment.Author} for article id = {createdComment.ArticleId}");
            return createdComment;
        }

        /// <summary>
        /// Обновить имеющийся комментарий
        /// </summary>
        /// <param name="comment">Обновляемый комментарий</param>
        public Comment Update(Comment comment)
        {
            Comment updatedComment = connection.QuerySingle<Comment>(UpdateQuery, comment);
            Log.Information($"Comment updated (Id={updatedComment.Id}) by {updatedComment.Author} for article id = {updatedComment.ArticleId}");
            return updatedComment;
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
    }
}