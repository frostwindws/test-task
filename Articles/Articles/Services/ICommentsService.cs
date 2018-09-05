using System.ServiceModel;
using Articles.Models;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы с комментариями к статьям
    /// </summary>
    [ServiceContract]
    public interface ICommentsService
    {
        /// <summary>
        /// Запрос списка комментариев к статье.
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Возвращает массив комментариев к указанной статье.</returns>
        [OperationContract]
        Comment[] GetForArticle(long id);

        /// <summary>
        /// Создание комментария
        /// </summary>
        /// <param name="comment">Создаваемый комментарий</param>
        /// <returns></returns>
        [OperationContract]
        long Create(Comment comment);

        /// <summary>
        /// Обновление имеющегося комментария.
        /// </summary>
        /// <param name="comment">Обновляемый комментарий</param>
        /// <returns></returns>
        [OperationContract]
        void Update(Comment comment);

        /// <summary>
        /// Удаление комментария
        /// </summary>
        /// <param name="id">Идентификатор удаляемого комментария</param>
        [OperationContract]
        void Delete(long id);
    }
}
