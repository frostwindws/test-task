using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArticlesClient.Models;

namespace ArticlesClient.Controls
{
    /// <summary>
    /// Interaction logic for CommentViewer.xaml
    /// </summary>
    public partial class CommentViewer : UserControl
    {
        /// <summary>
        /// Событие запроса на редактирование коментария.
        /// </summary>
        public event EventHandler<CommentView> DoCommentEdit;

        /// <summary>
        /// Событие запроса на удаление коментария.
        /// </summary>
        public event EventHandler<CommentView> DoCommentDelete;

        public CommentViewer()
        {
            InitializeComponent();
        }

        private void EditComment_OnClick(object sender, RoutedEventArgs e)
        {
            DoCommentEdit?.Invoke(this, DataContext as CommentView);
        }

        private void DeleteComment_OnClick(object sender, RoutedEventArgs e)
        {
            DoCommentDelete?.Invoke(this, DataContext as CommentView);
        }
    }
}
