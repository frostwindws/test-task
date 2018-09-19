using System;
using System.Collections.ObjectModel;
using ArticlesService;
using ArticlesWeb.Models;
using AutoMapper;

namespace ArticlesWeb.Commands.Articles
{
    /// <summary>
    /// Команда обработки оповещения о добавлении статьи.
    /// </summary>
    internal class CreateArticleCommand : IRequestCommand<ArticleDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении о добавлении статьи).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Добавленная статья.</param>
        public void Execute(ViewDataContainer viewData, ArticleDto record)
        {
            if (viewData.ArticlesList != null)
            {
                Article article = Mapper.Map<Article>(record);
                viewData.ArticlesList.Add(article);
                viewData.ArticlesList = new ObservableCollection<Article>(viewData.ArticlesList.OrderByDescending(c => c.Created));
            }
        }
    }
}