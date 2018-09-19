using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using ArticlesService;
using ArticlesWeb.Models;
using AutoMapper;

namespace ArticlesWeb.Clients.Wcf
{
    /// <summary>
    /// Комбинированный (WCF/Rabbit) репозиторий статей.
    /// </summary>
    public class ArticlesRepository : IRepository<Article>
    {
        /// <summary>
        /// Wcf сервис для запроса данных.
        /// </summary>
        private readonly IArticlesService articlesService;

        /// <summary>
        /// Конструктор репозитория статей.
        /// </summary>
        /// <param name="service">Используемый WCF сервис для запроса данных.</param>
        public ArticlesRepository(ArticlesServiceClient service)
        {
            articlesService = service;
        }

        /// <summary>
        /// Получить все статьи.
        /// </summary>
        /// <returns>Перечисление всех статей.</returns>
        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            var result = await articlesService.GetAllAsync();
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<Article[]>(result.Data);
        }

        /// <summary>
        /// Получить статью по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи.</param>
        /// <returns>Найденная статья, либо null  в случае отсутствия статьи с указанным идентификатором.</returns>
        public async Task<Article> GetAsync(long id)
        {
            var result = await articlesService.GetAsync(id);
            if (!result.Success)
            {
                throw new CommunicationException(result.Message);
            }

            return Mapper.Map<Article>(result.Data);
        }
    }
}
