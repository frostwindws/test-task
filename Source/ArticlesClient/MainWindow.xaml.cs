using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ArticlesClient.Clients.Wcf;
using ArticlesClient.Models;
using ArticlesClient.Utils;

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

        private DataClientsFactory ReaderFactory => ((App)Application.Current).ReaderFactory;

        private DataClientsFactory WriterFactory => ((App)Application.Current).WriterFactory;

        /// <summary>
        /// Конструктор основного окна приложения
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            viewData = (ViewDataContainer)DataContext;

            RequestArticlesList();
        }

        /// <summary>
        /// Запросить список статей
        /// </summary>
        private async void RequestArticlesList()
        {
            try
            {
                IsEnabled = false;
                using (var client = ReaderFactory.GetClient())
                {
                    viewData.ArticlesList = new ObservableCollection<ArticleView>(await client.Articles.GetAllAsync());
                }
            }
            catch (CommunicationException e)
            {
                MessageBox.Show($"Service error: {e.Message}.");
            }
            finally
            {
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Запросить данные отдельной статьи
        /// </summary>
        /// <param name="articleId">Идентификатор статьи</param>
        /// <returns></returns>
        private void RequestArticledata(long articleId)
        {
            DoSafeRequest(async () =>
            {
                try
                {
                    viewData.CurrentArticle = null;
                    ArticlesPanel.IsEnabled = false;
                    using (var client = ReaderFactory.GetClient())
                    {
                        viewData.CurrentArticle = await client.Articles.GetAsync(articleId);
                    }
                }
                finally
                {
                    ArticlesPanel.IsEnabled = true;
                }
            });
        }

        /// <summary>
        /// Обращение к клиенту данных со стандартной обработкой ошибок коммуникации
        /// </summary>
        /// <param name="request">Метод обращения</param>
        private async void DoSafeRequest(Func<Task> request)
        {
            try
            {
                await request.Invoke();
            }
            catch (CommunicationException e)
            {
                MessageBox.Show(e.Message, "Service error");
            }
        }

        private void ArticlesListButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Контекстом кнопки является статья
            ArticleView article = (ArticleView)((Button)sender).DataContext;
            RequestArticledata(article.Id);
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

        private void ArticleEditor_OnCommitEdit(object sender, ArticleView article)
        {
            DoSafeRequest(async () =>
                {
                    try
                    {
                        IsEnabled = false;
                        using (var client = WriterFactory.GetClient())
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
                                RequestArticledata(article.Id);
                            }
                        }
                    }
                    finally
                    {
                        IsEnabled = true;
                    }
                });
        }

        private void ArticleEditor_OnCancelEdit(object sender, ArticleView article)
        {
            viewData.EditableArticle = null;
        }

        private void ArticleViewer_OnDoArticleDelete(object sender, ArticleView article)
        {
            DoSafeRequest(async () =>
                {
                    try
                    {
                        MessageBoxResult result = MessageBox.Show($"Do you really want to delete article \"{article.Title}\"?", "Confirmation", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            IsEnabled = false;
                            using (var client = WriterFactory.GetClient())
                            {
                                await client.Articles.DeleteAsync(article);
                            }

                            long articleId = article.Id;
                            article = viewData.ArticlesList.FirstOrDefault(a => a.Id == articleId);
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
                });
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
            DoSafeRequest(async () =>
            {
                try
                {
                    MessageBoxResult result = MessageBox.Show("Do you really want to delete this comment?", "Confirmation", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        IsEnabled = false;
                        using (var client = WriterFactory.GetClient())
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
            });
        }

        private void CommentEditor_OnCommentCommit(object sender, CommentView comment)
        {
            DoSafeRequest(async () =>
                {
                    try
                    {
                        IsEnabled = false;
                        ObservableCollection<CommentView> commentsList = viewData.CurrentArticle.Comments;
                        using (var client = WriterFactory.GetClient())
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
                });
        }

        private void CommentEditor_OnCommentCancel(object sender, CommentView comment)
        {
            viewData.EditableComment = null;
        }

        private void RefreshArticle_OnClick(object sender, RoutedEventArgs e)
        {
            RequestArticlesList();
        }

        private void ArticleViewer_OnDoRefresh(object sender, ArticleView article)
        {
            RequestArticledata(article.Id);
        }
    }
}
