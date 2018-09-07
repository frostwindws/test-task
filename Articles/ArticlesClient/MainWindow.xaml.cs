using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient
{
    /// <summary>
    /// Основное окно приложения
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Контейнер данных для работы с окном
        /// </summary>
        private readonly ViewDataContainer viewData;

        /// <summary>
        /// Конструктор основного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            viewData = (ViewDataContainer)DataContext;

            InitializeModelsMapping();
            RequestArticlesList();
        }

        /// <summary>
        /// Инициализация маппинга моделей
        /// </summary>
        private void InitializeModelsMapping()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<ArticleData, ArticleView>();
                c.CreateMap<CommentData, CommentView>();
            });
        }

        /// <summary>
        /// Запросить список статей
        /// </summary>
        private void RequestArticlesList()
        {
            using (var articlesService = new ArticlesServiceClient())
            {
                ArticleData[] articles = articlesService.GetAll();
                viewData.ArticlesList = Mapper.Map<ObservableCollection<ArticleView>>(articles.OrderByDescending(a => a.Created));
            }

        }

        private void ArticlesListButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Контекстом кнопки является статья
            ArticleView article = (ArticleView)((Button)sender).DataContext;
            using (var articlesService = new ArticlesServiceClient())
            {
                viewData.CurrentArticle = Mapper.Map<ArticleView>(articlesService.Get(article.Id));
            }
        }

        private void CreateArticle_OnClick(object sender, RoutedEventArgs e)
        {
            var newArticle = new ArticleView { IsNew = true };
            viewData.EditableArticle = newArticle;
        }

        private void ArticleEditor_OnCommitEdit(object sender, ArticleView article)
        {
            using (var service = new ArticlesServiceClient())
            {
                // В зависимости от флага IsNew производится либо создание новой, либо обновление имеющейся статьи
                var data = Mapper.Map<ArticleData>(article);
                if (article.IsNew)
                {
                    long? createdId = service.Create(data);
                    if (!createdId.HasValue)
                    {
                        article.Id = createdId.Value;
                        article.IsNew = false;
                        viewData.ArticlesList.Insert(0, article);
                    }
                }
                else
                {
                    service.Update(data);
                    int index = viewData.ArticlesList.IndexOf(viewData.ArticlesList.First(a => a.Id == article.Id));
                    viewData.ArticlesList[index] = article;
                }
            }

            viewData.EditableArticle = null;
            viewData.CurrentArticle = article;
        }

        private void ArticleEditor_OnCancelEdit(object sender, ArticleView article)
        {
            viewData.EditableArticle = null;
        }

        private void ArticleViewer_OnDoArticleEdit(object sender, ArticleView article)
        {
            viewData.EditableArticle = new ArticleView
            {
                Id = article.Id,
                Author = article.Author,
                Content = article.Content,
                Created = article.Created
            };
        }

        private void ArticleViewer_OnDoArticleDelete(object sender, ArticleView article)
        {
            throw new System.NotImplementedException();
        }

        private void ArticleViewer_OnDoCommentAdd(object sender, ArticleView article)
        {
            viewData.EditableComment = new CommentView
                {
                    ArticleId = article.Id,
                    IsNew = true
                };
        }

        private void ArticleViewer_OnDoCommentEdit(object sender, CommentView comment)
        {
            viewData.EditableComment = new CommentView
            {
                Id = comment.Id,
                ArticleId = comment.ArticleId,
                Author = comment.Author,
                Content = comment.Content,
                Created = comment.Created
            };
        }

        private void ArticleViewer_OnDoCommentDelete(object sender, CommentView comment)
        {
            throw new System.NotImplementedException();
        }

        private void CommentEditor_OnCommentCommit(object sender, CommentView comment)
        {
            throw new System.NotImplementedException();
        }

        private void CommentEditor_OnCommentCancel(object sender, CommentView comment)
        {
            throw new System.NotImplementedException();
        }
    }
}
