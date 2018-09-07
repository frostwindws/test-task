using System.ServiceModel;
using Articles.Services.Models;

namespace Articles.Services
{
    /// <summary>
    /// Интерфейс сервиса статей
    /// </summary>
    [ServiceContract]
    public interface IArticlesService
    {
        /// <summary>
        /// Получить список статей.
        /// </summary>
        /// <returns>Возвращает массив всех имеющихся статей.</returns>
        [OperationContract]
        ArticleData[] GetAll();

        /// <summary>
        /// Получить отдельную статью
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Возвращает объект запрашиваемой статьи.</returns>
        [OperationContract]
        ArticleData Get(long id);

        /// <summary>
        /// Создать новую статью
        /// </summary>
        /// <param name="article">Объект создаваемой стаьи</param>
        [OperationContract]
        long? Create(ArticleData article);

        /// <summary>
        /// Обновить имеющуюся статью
        /// </summary>
        /// <param name="article">Объект обновляемой статьи</param>
        [OperationContract]
        void Update(ArticleData article);

        /// <summary>
        /// Удалить статью
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи</param>
        [OperationContract]
        void Delete(long id);
    }
}
