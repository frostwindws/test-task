using System;
using System.Windows;
using ArticlesClient.Models;

namespace ArticlesClient.Controls
{
    /// <summary>
    /// Форма редактировния статьи.
    /// </summary>
    public partial class ArticleEditor
    {
        /// <summary>
        /// Событие принятия редактирования статьи.
        /// </summary>
        public event EventHandler<ArticleView> CommitEdit;

        /// <summary>
        /// Событие отмены редактирования статьи.
        /// </summary>
        public event EventHandler<ArticleView> CancelEdit;

        /// <summary>
        /// Конструктор формы редактирования статьи.
        /// </summary>
        public ArticleEditor()
        {
            InitializeComponent();
        }

        private void SaveBase_OnClick(object sender, RoutedEventArgs e)
        {
            CommitEdit?.Invoke(this, DataContext as ArticleView);
        }

        private void CancelBase_OnClick(object sender, RoutedEventArgs e)
        {
            CancelEdit?.Invoke(this, DataContext as ArticleView);
        }
    }
}
