namespace Articles.Messaging
{
    /// <summary>
    /// Структура данных сообщения.
    /// </summary>
    public struct Message
    {
        /// <summary>
        /// Идентификатор сообщения.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Ключ очереди ожидания ответа.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Данные запроса.
        /// </summary>
        public byte[] Body { get; set; }
    }
}