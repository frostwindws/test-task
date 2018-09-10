using System;
using System.Windows;
using ArticlesClient.Models;

namespace ArticlesClient.Controls
{
    /// <summary>
    /// Interaction logic for CommentEditor.xaml
    /// </summary>
    public partial class CommentEditor
    {
        /// <summary>
        /// Событие принятия редактирования статьи
        /// </summary>
        public event EventHandler<CommentView> CommitEdit;

        /// <summary>
        /// Событие отмены редактирования статьи
        /// </summary>
        public event EventHandler<CommentView> CancelEdit;

        public CommentEditor()
        {
            InitializeComponent();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            CommitEdit?.Invoke(this, DataContext as CommentView);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            CancelEdit?.Invoke(this, DataContext as CommentView);
        }
    }
}
