using ArticlesService.Services.Models;
using System.ServiceModel;

namespace ArticlesService.Services
{
    /// <summary>
    /// Сервис работы с комментариями к статьям.
    /// </summary>
    [ServiceContract]
    public interface ICommentsService
    {
        /// <summary>
        /// Получить список комментариев.
        /// </summary>
        /// <returns>Возвращает массив всех имеющихся статей.</returns>
        [OperationContract]
        ResultDto<CommentDto[]> GetAll();

        /// <summary>
        /// Получить отдельный комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        /// <returns>Возвращает объект запрашиваемого комментария.</returns>
        [OperationContract]
        ResultDto<CommentDto> Get(long id);

        /// <summary>
        /// Запрос списка комментариев к статье.
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Возвращает массив комментариев к указанной статье.</returns>
        [OperationContract]
        ResultDto<CommentDto[]> GetForArticle(long id);

        /// <summary>
        /// Создание комментария.
        /// </summary>
        /// <param name="comment">Создаваемый комментарий.</param>
        /// <returns></returns>
        [OperationContract]
        ResultDto<CommentDto> Create(CommentDto comment);

        /// <summary>
        /// Обновление имеющегося комментария.
        /// </summary>
        /// <param name="comment">Обновляемый комментарий.</param>
        /// <returns></returns>
        [OperationContract]
        ResultDto<CommentDto> Update(CommentDto comment);

        /// <summary>
        /// Удаление комментария.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого комментария.</param>
        [OperationContract]
        ResultDto<CommentDto> Delete(long id);
    }
}
