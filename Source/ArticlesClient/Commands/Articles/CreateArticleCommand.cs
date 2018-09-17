﻿using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;
using System.Collections.ObjectModel;
using System.Linq;

namespace Articles.Services.Executors.Articles
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
                ArticleView article = Mapper.Map<ArticleView>(record);
                viewData.ArticlesList.Add(article);
                viewData.ArticlesList = new ObservableCollection<ArticleView>(viewData.ArticlesList.OrderByDescending(c => c.Created));
            }
        }
    }
}