using System;
using System.Linq;
using Articles.Models;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы с комментариями к статьям
    /// </summary>
    public class CommentsService : ICommentsService, IDisposable
    {
        /// <summary>
        /// Репозиторий для работы с комментариями
        /// </summary>
        protected ICommentsRepository Repository { get; set; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="repository">Используемый репозиторий</param>
        public CommentsService(ICommentsRepository repository)
        {
            Repository = repository;
        }

        /// <inheritdoc />
        public Comment[] GetForArticle(long articleid)
        {
            return Repository.GetForArticle(articleid).ToArray();
        }

        /// <inheritdoc />
        public long Create(Comment comment)
        {
            return Repository.Create(comment);
        }

        /// <inheritdoc />
        public void Update(Comment comment)
        {
            Repository.Update(comment);
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
