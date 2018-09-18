using System;
using System.Collections.Generic;
using System.Linq;
using Articles.Models;

namespace Articles.Services.Commands.Comments
{
    /// <summary>
    /// Команда на добавление комментария.
    /// </summary>
    public class CreateCommentCommand : IRequestCommand<Comment>
    {               
        /// <summary>
        /// Выполняемый метод (Добавление комментария в репозиторий).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Добавляемый комментарий</param>
        /// <param name="validator">Валидатор для проверки комментария.</param>
        /// <returns>Добавленный комментарий.</returns>
        public Comment Execute(IDataContext context, Comment record, IModelValidator<Comment> validator)
        {
            if (record == null)
            {
                throw new ArgumentNullException("The comment is empty");
            }

            IEnumerable<string> errors = validator.GetErrors(context, record);
            if (errors.Any())
            {
                throw new ArgumentException($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
            }
            else
            {
                var createdArticle = context.Comments.Create(record);
                context.Commit();
                return createdArticle;
            }
        }
    }
}