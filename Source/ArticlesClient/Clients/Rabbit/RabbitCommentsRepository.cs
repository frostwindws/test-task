using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Rabbit MQ репозиторий комментариев.
    /// </summary>
    class RabbitCommentsRepository : IRepository<CommentView>
    {
        private readonly RabbitRequestProvider provider;

        /// <summary>
        /// Конструктор репозитория комментариев.
        /// </summary>
        /// <param name="provider">Используемый провайдер для обращения к Rabbit.</param>
        public RabbitCommentsRepository(RabbitRequestProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Получить все комментарии
        /// Метод не используется.
        /// </summary>
        /// <returns>Полный список всех комментариев в системе.</returns>
        public Task<IEnumerable<CommentView>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить комментарий по его идентификатору
        /// Метод не используется.
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария.</param>
        /// <returns>Найденный комментарий, либо null  в случае отсутствия комментария с указанным идентификатором.</returns>
        public Task<CommentView> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Добавление нового комментария.
        /// </summary>
        /// <param name="record">Добавляемый комментарий.</param>
        /// <returns>Данные созданного комментария.</returns>
        public async Task<CommentView> AddAsync(CommentView record)
        {
            RabbitResult<CommentDto> result = await provider.SendRequest<RabbitResult<CommentDto>>(Operations.CreateComment, Mapper.Map<CommentDto>(record));
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
            RabbitResult<CommentDto> result = await provider.SendRequest<RabbitResult<CommentDto>>(Operations.UpdateComment, Mapper.Map<CommentDto>(record));
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
            RabbitResult<CommentDto> result = await provider.SendRequest<RabbitResult<CommentDto>>(Operations.DeleteComment, Mapper.Map<CommentDto>(record));
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }
        }
    }
}
