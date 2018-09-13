using ArticlesClient.Models;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Клиент обращения к Rabbit MQ.
    /// </summary>
    public class RabbitClient : IDataClient
    {
        private readonly RabbitRequestProvider provider;

        /// <summary>
        /// Репозиторий статей.
        /// </summary>
        public IRepository<ArticleView> Articles { get; set; }

        /// <summary>
        /// Репозиторий комментариев.
        /// </summary>
        public IRepository<CommentView> Comments { get; set; }

        /// <summary>
        /// Конструктор клиента обращения к Rabbit MQ.
        /// </summary>
        /// <param name="provider">Провайдер обращения к Rabbit.</param>
        public RabbitClient(RabbitRequestProvider provider)
        {
            this.provider = provider;
            Articles = new RabbitArticlesRepository(provider);
            Comments = new RabbitCommentsRepository(provider);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            provider?.Dispose();
        }
    }
}
