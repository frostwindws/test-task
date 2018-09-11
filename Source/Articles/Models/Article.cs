using System;
using System.Collections.Generic;
using Nelibur.Sword.Extensions;

namespace Articles.Models
{
    /// <summary>
    /// Модель статьи
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

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
        /// Валидация статьи
        /// </summary>
        /// <returns>Перечисление ошибок</returns>
        public IEnumerable<string> Validate()
        {
            var errors = new List<string>();
            this.ToOption()
                .Do(a => string.IsNullOrWhiteSpace(a.Title), a => errors.Add("Article title cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Author), a => errors.Add("Article author cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Content), a => errors.Add("Article content cant't be empty"));

            return errors;
        }

    }
}