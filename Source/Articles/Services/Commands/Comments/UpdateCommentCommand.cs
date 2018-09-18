using System;
using System.Collections.Generic;
using System.Linq;
using Articles.Models;

namespace Articles.Services.Commands.Comments
{
    /// <summary>
    /// Команда на обновление комментария.
    /// </summary>
    public class UpdateCommentCommand : IRequestCommand<Comment>
    {               
        /// <summary>
        /// Выполняемый метод (Обновление комментария в репозиторий).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Обновляемый комментарий</param>
        /// <param name="validator">Валидатор для проверки комментария.</param>
        /// <returns>Обновленный комментарий.</returns>
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
                var updatedComment = context.Comments.Update(record);
                context.Commit();
                return updatedComment;
            }
        }
    }
}