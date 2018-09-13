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
    /// Rabbit MQ репозиторий статей.
    /// </summary>
    class RabbitArticlesRepository : IRepository<ArticleView>
    {
        private readonly RabbitRequestProvider provider;

        /// <summary>
        /// Конструктор репозитория статей.
        /// </summary>
        /// <param name="provider">Используемый провайдер для обращения к Rabbit.</param>
        public RabbitArticlesRepository(RabbitRequestProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Получить все статьи
        /// Метод не используется.
        /// </summary>
        /// <returns>Перечисление всех статей.</returns>
        public Task<IEnumerable<ArticleView>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить статью по ее идентификатору
        /// Метод не используется.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи.</param>
        /// <returns>Найденная статья, либо null  в случае отсутствия статьи с указанным идентификатором.</returns>
        public Task<ArticleView> GetAsync(long id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Добавление новой статьи.
        /// </summary>
        /// <param name="record">Добавляемая статья.</param>
        /// <returns>Данные созданной статьи.</returns>
        public async Task<ArticleView> AddAsync(ArticleView record)
        {
            RabbitResult<ArticleDto> result = await provider.SendRequest<RabbitResult<ArticleDto>>(Operations.CreateArticle, Mapper.Map<ArticleDto>(record));
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<ArticleView>(result.Data);
        }

        /// <summary>
        /// Обновление существующей статьи.
        /// </summary>
        /// <param name="record">Данные статьи для ее обновления.</param>
        /// <returns>Обновленная статья.</returns>
        public async Task<ArticleView> UpdateAsync(ArticleView record)
        {
            RabbitResult<ArticleDto> result = await provider.SendRequest<RabbitResult<ArticleDto>>(Operations.UpdateArticle, Mapper.Map<ArticleDto>(record));
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<ArticleView>(result.Data);
        }

        /// <summary>
        /// Удаление существующей статьи.
        /// </summary>
        /// <param name="record">Удаляемая статья.</param>
        public async Task DeleteAsync(ArticleView record)
        {
            RabbitResult<ArticleDto> result = await provider.SendRequest<RabbitResult<ArticleDto>>(Operations.DeleteArticle, record);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }
        }
    }
}
