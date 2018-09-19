using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ArticlesWeb.Clients.Rabbit;
using ArticlesWeb.Models;
using AutoMapper;
using CommentsService;

namespace ArticlesWeb.Clients.Wcf
{
    /// <summary>
    /// Комбинированный (WCF/Rabbit) репозиторий комментариев.
    /// </summary>
    public class CommentsRepository : IRepository<Comment>
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
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            var result =  await commentsService.GetAllAsync();
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<Comment[]>(result.Data);
        }

        /// <summary>
        /// Получить комментарий по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария.</param>
        /// <returns>Найденный комментарий, либо null  в случае отсутствия комментария с указанным идентификатором.</returns>
        public async Task<Comment> GetAsync(long id)
        {
            var result = await commentsService.GetAsync(id);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<Comment>(result.Data);
        }
    }
}
