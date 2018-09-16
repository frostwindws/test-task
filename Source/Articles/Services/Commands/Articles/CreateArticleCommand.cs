using Articles.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Articles.Services.Executors.Articles
{
    /// <summary>
    /// Команда на добавление статьи.
    /// </summary>
    public class CreateArticleCommand : IRequestCommand<Article>
    {               
        /// <summary>
        /// Выполняемый метод (Добавление статьи в репозиторий).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Добавляемая статья</param>
        /// <param name="validator">Валидатор для проверки статьи.</param>
        /// <returns>Добавленная статья.</returns>
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
                var createdArticle = context.Articles.Create(record);
                context.Commit();
                return createdArticle;
            }
        }
    }
}