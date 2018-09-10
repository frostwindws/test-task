using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArticlesClient.Clients
{
    /// <summary>
    /// Интерфейс репозитория записей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Получить все записи
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получить запись по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор искомой записи</param>
        /// <returns>Данные искомой записи</returns>
        Task<T> GetAsync(long id);

        /// <summary>
        /// Добавление новой записи
        /// </summary>
        /// <param name="record">Данные добавляемой записи</param>
        /// <returns>Созданная запись</returns>
        Task<T> AddAsync(T record);

        /// <summary>
        /// Обновление имеющейся записи
        /// </summary>
        /// <param name="record">Обновляемые данные записи</param>
        /// <returns>Обновленная запись</returns>
        Task<T> UpdateAsync(T record);

        /// <summary>
        /// Удаление имеющейся записи
        /// </summary>
        /// <param name="record">Удаляемая запись</param>
        Task DeleteAsync(T record);
    }
}
