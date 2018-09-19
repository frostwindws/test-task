using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArticlesWeb.Clients.Wcf
{
    /// <summary>
    /// Интерфейс репозитория записей.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Получить все записи.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получить запись по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомой записи.</param>
        /// <returns>Данные искомой записи.</returns>
        Task<T> GetAsync(long id);
    }
}
