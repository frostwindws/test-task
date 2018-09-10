using System;
using System.Windows;
using ArticlesClient.Models;

namespace ArticlesClient.Controls
{
    /// <summary>
    /// Компонент просмотра статьи
    /// </summary>
    public partial class ArticleViewer
    {
        /// <summary>
        /// Событие запроса на редактирование статьи
        /// </summary>
        public event EventHandler<ArticleView> DoArticleEdit;

        /// <summary>
        /// Событие запроса на удаление статьи
        /// </summary>
        public event EventHandler<ArticleView> DoArticleDelete;

        /// <summary>
        /// Событие запроса на добавление коментария
        /// </summary>
        public event EventHandler<ArticleView> DoCommentAdd;

        /// <summary>
        /// Событие запроса на обновление статьи
        /// </summary>
        public event EventHandler<ArticleView> DoRefresh;

        /// <summary>
        /// Событие запроса на редактирование коментария
        /// </summary>
        public event EventHandler<CommentView> DoCommentEdit;

        /// <summary>
        /// Событие запроса на удаление коментария
        /// </summary>
        public event EventHandler<CommentView> DoCommentDelete;

        /// <summary>
        /// Конструктор компонента
        /// </summary>
        public ArticleViewer()
        {
            InitializeComponent();
        }

        private void EditArticle_OnClick(object sender, RoutedEventArgs e)
        {
            DoArticleEdit?.Invoke(this, DataContext as ArticleView);
        }

        private void DeleteArticle_OnClick(object sender, RoutedEventArgs e)
        {
            DoArticleDelete?.Invoke(this, DataContext as ArticleView);
        }

        private void AddComment_OnClick(object sender, RoutedEventArgs e)
        {
            DoCommentAdd?.Invoke(this, DataContext as ArticleView);
        }
        
        private void CommentViewer_OnDoCommentEdit(object sender, CommentView e)
        {
            DoCommentEdit?.Invoke(this, ((CommentViewer)sender).DataContext as CommentView);
        }

        private void CommentViewer_OnDoCommentDelete(object sender, CommentView e)
        {
            DoCommentDelete?.Invoke(this, ((CommentViewer)sender).DataContext as CommentView);
        }

        private void RefreshArticle_OnClick(object sender, RoutedEventArgs e)
        {
            DoRefresh?.Invoke(this, DataContext as ArticleView);
        }
    }
}
