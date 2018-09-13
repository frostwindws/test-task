using System;
using System.Runtime.Serialization;

namespace Articles.Services.Models
{
    /// <summary>
    /// Результат выполнения операции.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class ResultDto<T>
    {
        /// <summary>
        /// Флаг успеха выполнения.
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Передаваемые данные.
        /// </summary>
        [DataMember]
        public T Data { get; set; }

        /// <summary>
        /// Сообщение о результате выполнения операции.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }
}