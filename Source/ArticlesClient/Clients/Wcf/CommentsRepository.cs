using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ArticlesClient.Connected_Services.CommentsService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Clients.Wcf
{
    /// <summary>
    /// Wcf репозиторий комментариев.
    /// </summary>
    public class CommentsRepository : IRepository<CommentView>
    {
        /// <summary>
        /// Wcf сервис для запроса данных.
        /// </summary>
        private readonly ICommentsService commentsService;

        /// <summary>
        /// Конструктор репозитория комментариев.
        /// </summary>
        /// <param name="service">Используемый WCF сервис для запроса данных.</param>
        public CommentsRepository(ICommentsService service)
        {
            commentsService = service;
        }

        /// <summary>
        /// Получить все комментарии.
        /// </summary>
        /// <returns>Полный список всех комментариев в системе.</returns>
        public async Task<IEnumerable<CommentView>> GetAllAsync()
        {
            var result =  await commentsService.GetAllAsync();
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<CommentView[]>(result.Data);
        }

        /// <summary>
        /// Получить комментарий по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария.</param>
        /// <returns>Найденный комментарий, либо null  в случае отсутствия комментария с указанным идентификатором.</returns>
        public async Task<CommentView> GetAsync(long id)
        {
            var result = await commentsService.GetAsync(id);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<CommentView>(result.Data);
        }

        /// <summary>
        /// Добавление нового комментария.
        /// </summary>
        /// <param name="record">Добавляемый комментарий.</param>
        /// <returns>Данные созданного комментария.</returns>
        public async Task<CommentView> AddAsync(CommentView record)
        {
            var result = await commentsService.CreateAsync(Mapper.Map<CommentDto>(record));
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<CommentView>(result.Data);
        }

        /// <summary>
        /// Обновление существующего комментария.
        /// </summary>
        /// <param name="record">Данные комментария для его обновления.</param>
        /// <returns>Обновленный комментарий.</returns>
        public async Task<CommentView> UpdateAsync(CommentView record)
        {
            var result = await commentsService.UpdateAsync(Mapper.Map<CommentDto>(record));
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<CommentView>(result.Data);
        }

        /// <summary>
        /// Удаление существующегно комментария.
        /// </summary>
        /// <param name="record">Удаляемый комментарий.</param>
        public async Task DeleteAsync(CommentView record)
        {
            var result = await commentsService.DeleteAsync(record.Id);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }
        }
    }
}
