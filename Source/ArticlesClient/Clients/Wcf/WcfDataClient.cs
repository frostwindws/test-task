using System.ServiceModel;
using ArticlesClient.ArticlesService;
using ArticlesClient.CommentsService;
using ArticlesClient.Models;

namespace ArticlesClient.Clients.Wcf
{
    /// <summary>
    /// Клиент получения данных через WCF сервис
    /// </summary>
    public class WcfDataClient : IDataClient
    {
        private readonly IArticlesService articlesService;
        private readonly ICommentsService commentsService;

        /// <summary>
        /// Репозиторий статей
        /// </summary>
        public IRepository<ArticleView> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев
        /// </summary>
        public IRepository<CommentView> Comments { get; set; }

        /// <summary>
        /// Конструктор клиента данных
        /// </summary>
        public WcfDataClient() : this(new ArticlesServiceClient(), new CommentsServiceClient())
        {
        }

        /// <summary>
        /// Конструктор клиента данных c использованием кастомнымных сервисов
        /// (Для тестирования)
        /// </summary>
        /// <param name="articlesService">Сервис статей</param>
        /// <param name="commentsService">Сервис комментариев</param>
        public WcfDataClient(IArticlesService articlesService, ICommentsService commentsService)
        {
            this.articlesService = articlesService;
            this.commentsService = commentsService;
            Articles = new ArticlesRepository(articlesService);
            Comments = new CommentsRepository(commentsService);
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (articlesService is ClientBase<IArticlesService> articlesClientService)
            {
                articlesClientService.Close();
            }

            if (commentsService is ClientBase<ICommentsService> commentsClientService)
            {
                commentsClientService.Close();
            }
        }
    }
}
