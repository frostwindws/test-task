using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArticlesClient.Annotations;
using Nelibur.Sword.Extensions;

namespace ArticlesClient.Models
{
    /// <summary>
    /// Модель статьи для отображения.
    /// </summary>
    public sealed class ArticleView : INotifyPropertyChanged, IDataErrorInfo
    {
        private string title;
        private string author;
        private string content;

        /// <summary>
        /// Идентификатор.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Автор.
        /// </summary>
        public string Author
        {
            get => author;
            set
            {
                author = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Текст.
        /// </summary>
        public string Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Флаг нового объекта.
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Коллекция комментариев.
        /// </summary>
        public ObservableCollection<CommentView> Comments { get; set; }

        /// <summary>
        /// Событие обновления свойства модели представления.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Получение ошибки по имени свойства.
        /// </summary>
        /// <param name="columnName">Имя свойства.</param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                string error = null;
                columnName
                    .ToOption()
                    .Do(name => name == "Title" && string.IsNullOrWhiteSpace(Title), a => error = "Title is required")
                    .Do(name => name == "Author" && string.IsNullOrWhiteSpace(Author), a => error = "Author is required")
                    .Do(name => name == "Content" && string.IsNullOrWhiteSpace(Content), a => error = "Content is required");
                return error;

            }
        }

        public string Error => null;
    }
}
