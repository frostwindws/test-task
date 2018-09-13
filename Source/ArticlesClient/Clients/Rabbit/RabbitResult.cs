using System;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Результат выполнения операции
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RabbitResult<T>
    {
        /// <summary>
        /// Флаг успеха выполнения
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Передаваемые данные
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Сообщение о результате выполнения операции
        /// </summary>
        public string Message { get; set; }
    }
}