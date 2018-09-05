using System;
using System.Runtime.Serialization;

namespace Articles.Models
{
    /// <summary>
    /// Модель статьи
    /// </summary>
    [DataContract]
    public class Article
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DataMember]
        public DateTime Created { get; set; }

    }
}