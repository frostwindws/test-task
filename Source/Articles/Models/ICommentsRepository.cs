using System.Collections.Generic;

namespace Articles.Models
{
    /// <summary>
    /// Интерфейс репозитория коментариев
    /// </summary>
    public interface ICommentsRepository : IRepository<Comment>
    {
        /// <summary>
        /// Получить коментарии для статьи
        /// </summary>
        /// <param name="articleId">Идентификатор статьи</param>
        /// <returns>Возвращает список коментариев для указаной статьи</returns>
        IEnumerable<Comment> GetForArticle(long articleId);
    }
}