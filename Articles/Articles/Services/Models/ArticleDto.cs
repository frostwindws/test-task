using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Articles.Services.Models
{
    /// <summary>
    /// Модель статьи
    /// </summary>
    [DataContract]
    public class ArticleDto
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

        /// <summary>
        /// Набор комментариев к статье
        /// </summary>
        [DataMember]
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}