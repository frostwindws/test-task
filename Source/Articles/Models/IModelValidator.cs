using System.Collections.Generic;

namespace Articles.Models
{
    /// <summary>
    /// Интерфейс проверки модели.
    /// </summary>
    /// <typeparam name="T">Класс модели.</typeparam>
    public interface IModelValidator<T>
    {
        /// <summary>
        /// Получение ошибок модели.
        /// </summary>
        /// <param name="context">Используемый контекст данных.</param>
        /// <param name="record">Проверямеая запись.</param>
        /// <returns>Перечисление ошибок модели.</returns>
        IEnumerable<string> GetErrors(IDataContext context, T record);
    }
}
