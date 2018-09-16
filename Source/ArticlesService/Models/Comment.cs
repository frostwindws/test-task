using System;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ArticlesService.Models
{
    /// <summary>
    /// Модель комментария к статье.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Идентификатор статьи.
        /// </summary>
        public virtual long ArticleId { get; set; }

        /// <summary>
        /// Автор.
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// Текст.
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public virtual DateTime Created { get; set; }


        public virtual Article Article { get; set; }
    }
}