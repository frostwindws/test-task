using System.Collections.Generic;

namespace Articles.Models
{
    /// <summary>
    /// Интерфейс репозитория комментариев.
    /// </summary>
    public interface ICommentsRepository : IRepository<Comment>
    {
        /// <summary>
        /// Получить комментарии для статьи.
        /// </summary>
        /// <param name="articleId">Идентификатор статьи.</param>
        /// <returns>Возвращает список комментариев для указаной статьи.</returns>
        IEnumerable<Comment> GetForArticle(long articleId);
    }
}