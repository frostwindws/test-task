using System;
using System.Collections.Generic;
using Nelibur.Sword.Extensions;

namespace Articles.Models
{
    /// <summary>
    /// Валидатор соделей статей.
    /// </summary>
    public class CommentsValidator : IModelValidator<Comment>
    {
        /// <summary>
        /// Получение списка ошибок.
        /// </summary>
        /// <param name="context">Контекст данных.</param>
        /// <param name="record">Проверяемая запись.</param>
        /// <returns>Перечисление ошибок.</returns>
        public IEnumerable<string> GetErrors(IDataContext context, Comment record)
        {
            var errors = new List<string>();
            record
                .ToOption()
                .DoOnEmpty(() => errors.Add("Comment is empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Author), a => errors.Add("Comment author cant't be empty"))
                .Do(a => string.IsNullOrWhiteSpace(a.Content), a => errors.Add("Comment content cant't be empty"));

            return errors;
        }
    }
}