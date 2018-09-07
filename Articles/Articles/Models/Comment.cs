using System;

namespace Articles.Models
{
    /// <summary>
    /// Модель комментария к статье
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор статьи
        /// </summary>
        public long ArticleId { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }
    }
}