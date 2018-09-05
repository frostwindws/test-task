using System.Collections.Generic;

namespace Articles.Models
{
    /// <summary>
    /// Интерфейс репозитория записей типа <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">Тип записей.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Получение полного списка всех записей
        /// </summary>
        /// <returns>Возвращает список всех записей, имеющихся в репозитарии</returns>
        IEnumerable<T> GetCollection();

        /// <summary>
        /// Найти запись по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор записи.</param>
        /// <returns>
        /// Возвращает найденную запись. 
        /// Если записи с указанным идентификатором нет, возвращает null.
        /// </returns>
        T Find(long id);

        /// <summary>
        /// Создать новую запись
        /// </summary>
        /// <param name="record">Объект создаваемой записи.</param>
        /// <returns>Возвращает идентификатор созданной записи.</returns>
        long Create(T record);

        /// <summary>
        /// Обновить имеющуюся запись.
        /// </summary>
        /// <param name="record">Обновляемая запись.</param>
        void Update(T record);

        /// <summary>
        /// Удалить запись.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой записи.</param>
        void Delete(long id);
    }
}
