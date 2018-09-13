using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArticlesClient.Annotations;

namespace ArticlesClient.Models
{
    /// <summary>
    /// Контейнер даннях для отображения в окне приложения.
    /// </summary>
    internal sealed class ViewDataContainer : INotifyPropertyChanged
    {
        private ObservableCollection<ArticleView> articlesList;
        private ArticleView currentArticle;
        private ArticleView editableArticle;
        private CommentView editableComment;

        /// <summary>
        /// Коллекция отображаемых статей.
        /// </summary>
        public ObservableCollection<ArticleView> ArticlesList
        {
            get => articlesList;
            set
            {
                articlesList = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Отображаемая в основном окне статья.
        /// </summary>
        public ArticleView CurrentArticle
        {
            get => currentArticle;
            set
            {
                // Допонительно производится сброс редактирования
                EditableArticle = null;
                EditableComment = null;
                currentArticle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Редактируемая статья.
        /// </summary>
        public ArticleView EditableArticle
        {
            get => editableArticle;
            set
            {
                // При редактировании статьи производится сброс редактирования комментария
                EditableComment = null;
                editableArticle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Редактируемыей комментарий.
        /// </summary>
        public CommentView EditableComment
        {
            get => editableComment;
            set
            {
                editableComment = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Событие обновления свойства объекта.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
