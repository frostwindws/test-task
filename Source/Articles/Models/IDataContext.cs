using System;

namespace Articles.Models
{
    /// <summary>
    /// Базовый абстрактный контекст работы со статьями комментариями
    /// </summary>
    public interface IDataContext: IDisposable
    {
        /// <summary>
        /// Репозиторий статей
        /// </summary>
        IArticlesRepository Articles { get; }

        /// <summary>
        /// Репозиторий коментариев
        /// </summary>
        ICommentsRepository Comments { get; }
    }
}
