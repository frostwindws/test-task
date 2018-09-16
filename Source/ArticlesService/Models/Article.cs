using System;
using System.Collections.Generic;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace ArticlesService.Models
{
    /// <summary>
    /// Модель статьи.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public virtual long Id { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public virtual string Title { get; set; }

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

        /// <summary>
        /// Комментарии к статье
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; }
    }
}