namespace ArticlesService.Messaging.Converters
{
    /// <summary>
    /// Интерфейс конвертера тела сообщения.
    /// </summary>
    public interface IMessageBodyConverter
    {
        /// <summary>
        /// Чтение тела сообщения.
        /// </summary>
        /// <typeparam name="T">Целевой тип.</typeparam>
        /// <param name="body">Тело сообщения.</param>
        /// <returns>Сформированный объект данных.</returns>
        T FromBody<T>(byte[] body);

        /// <summary>
        /// Конвертация данных в тело сообщения.
        /// </summary>
        /// <param name="data">Исходные данные.</param>
        /// <returns>Тело сообщения.</returns>
        byte[] ToBody(object data);
    }
}