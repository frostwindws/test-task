﻿using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using System.Linq;

namespace Articles.Services.Executors.Articles
{
    /// <summary>
    /// Команда обработки оповещения об обновлении статьи.
    /// </summary>
    internal class UpdateArticleCommand : IRequestCommand<ArticleDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении об обновлении статьи).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Обновленная статья.</param>
        public void Execute(ViewDataContainer viewData, ArticleDto record)
        {
            if (viewData.ArticlesList != null)
            {
                ArticleView articleFromList = viewData.ArticlesList.FirstOrDefault(a => a.Id == record.Id);
                if (articleFromList != null)
                {
                    // Для списка интересен только заголовок
                    articleFromList.Title = record.Title;
                }
            }

            if (viewData.CurrentArticle?.Id == record.Id)
            {
                viewData.CurrentArticle.Title = record.Title;
                viewData.CurrentArticle.Content = record.Content;
            }
        }
    }
}