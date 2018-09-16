using System;

namespace Articles.Models
{
    /// <summary>
    /// Базовый абстрактный контекст работы со статьями комментариями.
    /// </summary>
    public interface IDataContext: IDisposable
    {
        /// <summary>
        /// Репозиторий статей.
        /// </summary>
        IArticlesRepository Articles { get; }

        /// <summary>
        /// Репозиторий комментариев.
        /// </summary>
        ICommentsRepository Comments { get; }
        
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        void Commit();

        /// <summary>
        /// Отмена изменений
        /// </summary>
        void Rollback();
    }
}
