using System;
using System.Collections.Generic;
using Nelibur.Sword.Extensions;

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

        /// <summary>
        /// Валидация комментария
        /// </summary>
        /// <returns>Перечисление ошибок</returns>
        public IEnumerable<string> Validate()
        {
            var errors = new List<string>();
            this.ToOption()
                .Do(a => a.ArticleId <= 0, a => errors.Add("Comment doesn't have article link"))
                .Do(a => string.IsNullOrWhiteSpace(a.Author), a => errors.Add("Comment author cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Content), a => errors.Add("Comment content cant't be empty"));

            return errors;
        }
    }
}