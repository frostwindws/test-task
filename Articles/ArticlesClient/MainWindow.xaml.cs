using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ArticlesClient.ArticlesService;
using ArticlesClient.Clients;
using ArticlesClient.Models;
using ArticlesClient.Utils;
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
            AutomapHelper.InitMapping();

            viewData = (ViewDataContainer)DataContext;
            RequestArticlesList();
        }

        /// <summary>
        /// Запросить список статей
        /// </summary>
        private async void RequestArticlesList()
        {
            using (var client = AutofacHelper.Resolve<IDataClient>())
            {
                viewData.ArticlesList = new ObservableCollection<ArticleView>(await client.Articles.GetAllAsync());
            }
        }

        /// <summary>
        /// Запросить данные отдельной статьи
        /// </summary>
        /// <param name="articleId">Идентификатор статьи</param>
        /// <returns></returns>
        private async Task RequestArticledata(long articleId)
        {
            viewData.CurrentArticle = null;
            ArticlesPanel.IsEnabled = false;
            try
            {
                using (var client = AutofacHelper.Resolve<IDataClient>())
                {
                    viewData.CurrentArticle = await client.Articles.GetAsync(articleId);
                }
            }
            finally
            {
                ArticlesPanel.IsEnabled = true;
            }
        }

        private async void ArticlesListButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Контекстом кнопки является статья
            ArticleView article = (ArticleView)((Button)sender).DataContext;
            await RequestArticledata(article.Id);
        }

        private void CreateArticle_OnClick(object sender, RoutedEventArgs e)
        {
            var newArticle = new ArticleView { IsNew = true };
            viewData.EditableArticle = newArticle;
        }

        private void ArticleViewer_OnDoArticleEdit(object sender, ArticleView article)
        {
            viewData.EditableArticle = new ArticleView
            {
                Id = article.Id,
                Title = article.Title,
                Author = article.Author,
                Content = article.Content,
                Created = article.Created
            };
        }

        private async void ArticleEditor_OnCommitEdit(object sender, ArticleView article)
        {
            try
            {
                IsEnabled = false;
                using (var client = AutofacHelper.Resolve<IDataClient>())
                {
                    if (article.IsNew)
                    {
                        article = await client.Articles.AddAsync(article);
                        viewData.ArticlesList.Insert(0, article);
                        viewData.CurrentArticle = article;
                    }
                    else
                    {
                        article = await client.Articles.UpdateAsync(article);
                        int index = viewData.ArticlesList.IndexOf(viewData.ArticlesList.First(a => a.Id == article.Id));

                        if (index >= 0)
                        {
                            viewData.ArticlesList[index] = article;
                        }

                        // Необходима дополнительная подгрузка комментариев
                        viewData.CurrentArticle = await client.Articles.GetAsync(article.Id);
                    }
                }

            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void ArticleEditor_OnCancelEdit(object sender, ArticleView article)
        {
            viewData.EditableArticle = null;
        }

        private async void ArticleViewer_OnDoArticleDelete(object sender, ArticleView article)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show($"Do you really want to delete article \"{article.Title}\"?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    IsEnabled = false;
                    using (var client = AutofacHelper.Resolve<IDataClient>())
                    {
                        await client.Articles.DeleteAsync(article);
                    }

                    article = viewData.ArticlesList.FirstOrDefault(a => a.Id == article.Id);
                    if (article != null)
                    {
                        viewData.ArticlesList.Remove(article);
                    }

                    viewData.CurrentArticle = null;
                }
            }
            finally
            {
                IsEnabled = true;
            }
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

        private async void ArticleViewer_OnDoCommentDelete(object sender, CommentView comment)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to delete this comment?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    IsEnabled = false;
                    using (var client = AutofacHelper.Resolve<IDataClient>())
                    {
                        await client.Comments.DeleteAsync(comment);
                    }

                    viewData.CurrentArticle.Comments.Remove(comment);
                }
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private async void CommentEditor_OnCommentCommit(object sender, CommentView comment)
        {
            try
            {
                IsEnabled = false;
                ObservableCollection<CommentView> commentsList = viewData.CurrentArticle.Comments;
                using (var client = AutofacHelper.Resolve<IDataClient>())
                {
                    if (comment.IsNew)
                    {
                        comment = await client.Comments.AddAsync(comment);
                        commentsList.Add(comment);
                    }
                    else
                    {
                        comment = await client.Comments.UpdateAsync(comment);
                        int index = commentsList.IndexOf(commentsList.First(a => a.Id == comment.Id));
                        if (index >= 0)
                        {
                            commentsList[index] = comment;
                        }
                    }
                }

                viewData.EditableComment = null;
            }
            finally
            {
                IsEnabled = true;
            }
        }

        private void CommentEditor_OnCommentCancel(object sender, CommentView comment)
        {
            viewData.EditableComment = null;
        }

        private void RefreshArticle_OnClick(object sender, RoutedEventArgs e)
        {
            RequestArticlesList();
        }

        private async void ArticleViewer_OnDoRefresh(object sender, ArticleView article)
        {
            await RequestArticledata(article.Id);
        }
    }
}
