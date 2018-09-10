using System;
using System.Collections.ObjectModel;

namespace ArticlesClient.Models
{
    /// <summary>
    /// Модель статьи для отображения
    /// </summary>
    public class ArticleView
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Флаг нового объекта
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Коллекция комментариев
        /// </summary>
        public ObservableCollection<CommentView> Comments { get; set; }
    }
}
