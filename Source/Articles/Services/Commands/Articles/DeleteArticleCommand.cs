using Articles.Models;
using System;

namespace Articles.Services.Executors.Articles
{
    /// <summary>
    /// Команда на удаление статьи.
    /// </summary>
    public class DeleteArticleCommand : IRequestCommand<Article>
    {
        /// <summary>
        /// Выполняемый метод (Удаление статьи из репозитория).
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Удаляемая статья</param>
        /// <param name="validator">Валидатор для проверки статьи.</param>
        /// <returns>Возвращает пустой результат.</returns>
        public Article Execute(IDataContext context, Article record, IModelValidator<Article> validator)
        {
            if (record == null)
            {
                throw new ArgumentNullException("The article is empty");
            }

            context.Articles.Delete(record.Id);
            context.Commit();
            return null;
        }
    }
}