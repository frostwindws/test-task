using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArticlesClient.Annotations;
using Nelibur.Sword.Extensions;

namespace ArticlesClient.Models
{
    /// <summary>
    /// Модель комментария к статье для отображения
    /// </summary>
    public sealed class CommentView : INotifyPropertyChanged, IDataErrorInfo
    {
        private string author;
        private string content;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор статьи
        /// </summary>
        public long ArticleId { get; set; }

        /// <summary>
        /// Автор
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
        /// Текст
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
        /// Флаг нового объекта
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Событие обновления свойства модели представления
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                string error = null;
                columnName
                    .ToOption()
                    .Do(name => name == "Author" && string.IsNullOrWhiteSpace(Author), a => error = "Author is required")
                    .Do(name => name == "Content" && string.IsNullOrWhiteSpace(Content), a => error = "Content is required");
                return error;

            }
        }

        public string Error => null;

    }
}
