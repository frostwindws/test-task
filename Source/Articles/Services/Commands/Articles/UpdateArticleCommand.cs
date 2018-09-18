using System;
using System.Collections.Generic;
using System.Linq;
using Articles.Models;

namespace Articles.Services.Commands.Articles
{
    /// <summary>
    /// Команда на обновление статьи.
    /// </summary>
    public class UpdateArticleCommand : IRequestCommand<Article>
    {               
        /// <summary>
        /// Выполняемый метод (Обновление статьи в репозиторий).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Добавляемая статья</param>
        /// <param name="validator">Валидатор для проверки статьи.</param>
        /// <returns>Обновленная статья.</returns>
        public Article Execute(IDataContext context, Article record, IModelValidator<Article> validator)
        {
            if (record == null)
            {
                throw new ArgumentNullException("The article is empty");
            }

            IEnumerable<string> errors = validator.GetErrors(context, record);
            if (errors.Any())
            {
                throw new ArgumentException($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
            }
            else
            {
                var updatedArticle = context.Articles.Update(record);
                context.Commit();
                return updatedArticle;
            }
        }
    }
}