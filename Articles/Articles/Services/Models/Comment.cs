using System;
using System.Runtime.Serialization;

namespace Articles.Services.Models
{
    /// <summary>
    /// Модель комментария к статье
    /// </summary>
    [DataContract]
    public class CommentData
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Идентификатор статьи
        /// </summary>
        [DataMember]
        public long ArticleId { get; set; }

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