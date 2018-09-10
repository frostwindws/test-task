using System.Collections.Generic;
using System.Threading.Tasks;
using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;

namespace ArticlesClient.Clients.Wcf
{
    /// <summary>
    /// WCF репозиторий статей
    /// </summary>
    public class ArticlesRepository : IRepository<ArticleView>
    {
        /// <summary>
        /// Wcf сервис для запроса данных
        /// </summary>
        private readonly IArticlesService articlesService;

        /// <summary>
        /// Конструктор репозитория статей
        /// </summary>
        /// <param name="service">Используемый WCF сервис для запроса данных</param>
        public ArticlesRepository(IArticlesService service)
        {
            articlesService = service;
        }

        /// <summary>
        /// Получить все статьи
        /// </summary>
        /// <returns>Перечисление всех статей</returns>
        public async Task<IEnumerable<ArticleView>> GetAllAsync()
        {
            return Mapper.Map<ArticleView[]>(await articlesService.GetAllAsync());
        }

        /// <summary>
        /// Получить статью по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи</param>
        /// <returns>Найденная статья, либо null  в случае отсутствия статьи с указанным идентификатором</returns>
        public async Task<ArticleView> GetAsync(long id)
        {
            return Mapper.Map<ArticleView>(await articlesService.GetAsync(id));
        }

        /// <summary>
        /// Добавление новой статьи
        /// </summary>
        /// <param name="record">Добавляемая статья</param>
        /// <returns>Данные созданной статьи</returns>
        public async Task<ArticleView> AddAsync(ArticleView record)
        {
            ArticleDto createdArticle =  await articlesService.CreateAsync(Mapper.Map<ArticleDto>(record));
            if (createdArticle != null)
            {
                return Mapper.Map<ArticleView>(createdArticle);
            }

            return null;
        }

        /// <summary>
        /// Обновление существующей статьи
        /// </summary>
        /// <param name="record">Данные статьи для ее обновления</param>
        /// <returns>Обновленная статья</returns>
        public async Task<ArticleView> UpdateAsync(ArticleView record)
        {
            ArticleDto updatedArticle = await articlesService.UpdateAsync(Mapper.Map<ArticleDto>(record));
            if (updatedArticle != null)
            {
                return Mapper.Map<ArticleView>(updatedArticle);
            }

            return null;
        }

        /// <summary>
        /// Удаление существующей статьи
        /// </summary>
        /// <param name="record">Удаляемая статья</param>
        public async Task DeleteAsync(ArticleView record)
        {
            await articlesService.DeleteAsync(record.Id);
        }
    }
}
