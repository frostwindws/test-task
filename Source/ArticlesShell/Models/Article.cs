using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArticlesShell.Models
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
    }
}
