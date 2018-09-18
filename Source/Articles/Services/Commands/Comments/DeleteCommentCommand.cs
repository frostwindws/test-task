using System;
using Articles.Models;

namespace Articles.Services.Commands.Comments
{
    /// <summary>
    /// Команда на удаление комментария.
    /// </summary>
    public class DeleteCommentCommand : IRequestCommand<Comment>
    {
        /// <summary>
        /// Выполняемый метод (Удаление комментария из репозитория).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Удаляемый комментарий</param>
        /// <param name="validator">Валидатор для проверки комментария.</param>
        /// <returns>Возвращает пустой результат.</returns>
        public Comment Execute(IDataContext context, Comment record, IModelValidator<Comment> validator)
        {
            if (record == null)
            {
                throw new ArgumentNullException("The comment is empty");
            }

            context.Comments.Delete(record.Id);
            context.Commit();
            return record;
        }
    }
}