using System;
using System.Collections.Generic;
using Nelibur.Sword.Extensions;

namespace Articles.Models
{
    /// <summary>
    /// Валидатор соделей статей
    /// </summary>
    public class ArticlesValidator : IModelValidator<Article>
    {
        /// <summary>
        /// Получение списка ошибок
        /// </summary>
        /// <param name="repository">Используемый ропозиторий</param>
        /// <param name="record">Проверяемая запись</param>
        /// <returns>Перечисление ошибок</returns>
        public IEnumerable<string> GetErrors(IRepository<Article> repository, Article record)
        {
            var errors = new List<string>();
            record
                .ToOption()
                .DoOnEmpty(() => errors.Add("Article is empty"))
                .Do(a => repository.Exists("Title", a.Title), a => errors.Add("Article title should be unique"))
                .Do(a => string.IsNullOrWhiteSpace(a.Title), a => errors.Add("Article title cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Author), a => errors.Add("Article author cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Content), a => errors.Add("Article content cant't be empty"));

            return errors;
        }
    }
}