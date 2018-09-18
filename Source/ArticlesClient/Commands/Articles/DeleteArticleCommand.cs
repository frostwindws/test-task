using System;
using System.Linq;
using ArticlesClient.Connected_Services.ArticlesService;
using ArticlesClient.Models;

namespace ArticlesClient.Commands.Articles
{
    /// <summary>
    /// Команда обработки оповещения об удалении статьи.
    /// </summary>
    internal class DeleteArticleCommand : IRequestCommand<ArticleDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении об удалении статьи).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Удаленная статья.</param>
        public void Execute(ViewDataContainer viewData, ArticleDto record)
        {
            if (viewData.ArticlesList != null)
            {
                ArticleView articleFromList = viewData.ArticlesList.FirstOrDefault(a => a.Id == record.Id);
                viewData.ArticlesList.Remove(articleFromList);
            }
        }
    }
}