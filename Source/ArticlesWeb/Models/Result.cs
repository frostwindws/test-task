namespace ArticlesWeb.Models
{
    /// <summary>
    /// Модель ответа от сервера.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Конструктор экземпляра модели
        /// </summary>
        /// <param name="error">Текст ошибки</param>
        public Result(string error = null)
        {
            Success = string.IsNullOrEmpty(error);
            Message = error;
        }

        /// <summary>
        /// Флаг успешной обработки запроса.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Текст сообщения сервера
        /// </summary>
        public string Message { get; set; }
    }
}
