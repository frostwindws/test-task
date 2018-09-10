using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArticlesClient.CommentsService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Clients.Wcf
{
    /// <summary>
    /// Wcf репозиторий комментариев
    /// </summary>
    public class CommentsRepository : IRepository<CommentView>
    {
        /// <summary>
        /// Wcf сервис для запроса данных
        /// </summary>
        private readonly ICommentsService commentsService;

        /// <summary>
        /// Конструктор репозитория комментариев
        /// </summary>
        /// <param name="service">Используемый WCF сервис для запроса данных</param>
        public CommentsRepository(ICommentsService service)
        {
            commentsService = service;
        }

        /// <summary>
        /// Получить все комментарии
        /// </summary>
        /// <returns>Полный список всех комментариев в системе</returns>
        public async Task<IEnumerable<CommentView>> GetAllAsync()
        {
            return Mapper.Map<CommentView[]>(await commentsService.GetAllAsync());
        }

        /// <summary>
        /// Получить комментарий по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария</param>
        /// <returns>Найденный комментарий, либо null  в случае отсутствия комментария с указанным идентификатором</returns>
        public async Task<CommentView> GetAsync(long id)
        {
            return Mapper.Map<CommentView>(await commentsService.GetAsync(id));
        }

        /// <summary>
        /// Добавление нового комментария
        /// </summary>
        /// <param name="record">Добавляемый комментарий</param>
        /// <returns>Данные созданного комментария</returns>
        public async Task<CommentView> AddAsync(CommentView record)
        {
            CommentDto createdComment = await commentsService.CreateAsync(Mapper.Map<CommentDto>(record));
            if (createdComment != null)
            {
                return Mapper.Map<CommentView>(createdComment);
            }

            return null;
        }

        /// <summary>
        /// Обновление существующего комментария
        /// </summary>
        /// <param name="record">Данные комментария для его обновления</param>
        /// <returns>Обновленный комментарий</returns>
        public async Task<CommentView> UpdateAsync(CommentView record)
        {
            CommentDto updatedComment = await commentsService.UpdateAsync(Mapper.Map<CommentDto>(record));
            if (updatedComment != null)
            {
                return Mapper.Map<CommentView>(updatedComment);
            }

            return null;
        }

        /// <summary>
        /// Удаление существующегно комментария
        /// </summary>
        /// <param name="record">Удаляемый комментарий</param>
        public async Task DeleteAsync(CommentView record)
        {
            await commentsService.DeleteAsync(record.Id);
        }
    }
}
