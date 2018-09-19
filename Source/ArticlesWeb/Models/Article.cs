using System;
using System.Collections.Generic;

namespace ArticlesWeb.Models
{
    /// <summary>
    /// Модель статьи.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Автор.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Текст.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Комментарии к статье.
        /// </summary>
        public ICollection<Comment> Comments { get; set; }
    }
}