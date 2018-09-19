using System;
using System.Threading.Tasks;
using ArticlesService;
using ArticlesWeb.Models;
using CommentsService;

namespace ArticlesWeb.Clients.Wcf
{
    /// <summary>
    /// Клиент чтения данных через WCF сервис и обновления RabbitMQ.
    /// </summary>
    public class WcfDataContext : IReadContext
    {
        private readonly ArticlesServiceClient articlesService;
        private readonly CommentsServiceClient commentsService;

        /// <summary>
        /// Репозиторий статей.
        /// </summary>
        public IRepository<Article> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев.
        /// </summary>
        public IRepository<Comment> Comments { get; set; }

        /// <summary>
        /// Конструктор клиента данных c использованием кастомнымных сервисов
        /// </summary>
        public WcfDataContext() : this(new ArticlesServiceClient(), new CommentsServiceClient())
        {
        }

        /// <summary>
        /// Конструктор клиента данных c использованием кастомнымных сервисов
        /// (Для тестирования).
        /// </summary>
        /// <param name="articlesService">Сервис статей.</param>
        /// <param name="commentsService">Сервис комментариев.</param>
        public WcfDataContext(ArticlesServiceClient articlesService, CommentsServiceClient commentsService)
        {
            this.articlesService = articlesService;
            this.commentsService = commentsService;
            Articles = new ArticlesRepository(articlesService);
            Comments = new CommentsRepository(commentsService);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            Task.WaitAll(articlesService?.CloseAsync(), commentsService?.CloseAsync());
        }
    }
}
