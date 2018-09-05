using System;
using System.Linq;
using Articles.Models;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы со статьями
    /// </summary>
    public class ArticlesService : IArticlesService, IDisposable
    {
        /// <summary>
        /// Репозиторий для работы со статьями
        /// </summary>
        protected IArticlesRepository Repository { get; set; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="repository">Используемый репозиторий</param>
        public ArticlesService(IArticlesRepository repository)
        {
            Repository = repository;
        }


        /// <inheritdoc />
        public Article[] GetAll()
        {
            return Repository.GetCollection().ToArray();
        }

        /// <inheritdoc />
        public Article Get(long id)
        {
            return Repository.Find(id);
        }

        /// <inheritdoc />
        public long Create(Article article)
        {
            return Repository.Create(article);
        }

        /// <inheritdoc />
        public void Update(Article article)
        {
            Repository.Update(article);
        }

        /// <inheritdoc />
        public void Delete(long id)
        {
            Repository.Delete(id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Repository?.Dispose();
        }
    }
}
