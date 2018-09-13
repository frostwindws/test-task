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
        /// <param name="repository">Используемый репозиторий.</param>
        /// <param name="record">Проверямеая запись.</param>
        /// <returns></returns>
        IEnumerable<string> GetErrors(IRepository<T> repository, T record);
    }
}
