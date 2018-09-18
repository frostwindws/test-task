using ArticlesShell.Conververters;
using ArticlesShell.Models;
using ArticlesShell.Rabbit;
using System;
using System.Management.Automation;
using RabbitMQ.Client;

namespace ArticlesShell.Cmdlets
{
    /// <summary>
    /// Командлет удаления статьи
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, nameof(Article))]
    public class DeleteArticle : Cmdlet, IDisposable
    {
        /// <summary>
        /// Адрес шины RabbitQM по умолчанию
        /// </summary>
        private const string DefaultRabbitUri = "amqp://guest:guest@localhost:5672/";

        /// <summary>
        /// Имя очереди по умолчанию
        /// </summary>
        private const string DefaultQueue = "articles-requests";

        /// <summary>
        /// Клиент подключения к RabbitMQ
        /// </summary>
        private RabbitClient rabbitClient;

        /// <summary>
        /// Параметр автора статьи
        /// </summary>
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Mandatory = true, HelpMessage = "Article identity")]
        public long ArticleId { get; set; }

        /// <summary>
        /// Параметр адреса шины RabbitQM
        /// </summary>
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "RabbitQM Uri (default: \""+ DefaultRabbitUri + "\")")]
        public string RabbitUri { get; set; }

        /// <summary>
        /// Параметр используемой для запросов очереди
        /// </summary>
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "RabbitQM queue name (default: \"" + DefaultQueue + "\"")]
        public string Queue { get; set; }

        /// <summary>
        /// Запуск обработки
        /// </summary>
        protected override void BeginProcessing()
        {
            string rabbitUri = string.IsNullOrWhiteSpace(RabbitUri) ? DefaultRabbitUri : RabbitUri;
            string queue = string.IsNullOrWhiteSpace(Queue) ? DefaultQueue : Queue;
            var factory = new ConnectionFactory { Uri = new Uri(rabbitUri) };
            string applicationId = Guid.NewGuid().ToString();
            rabbitClient = new RabbitClient(factory.CreateConnection(), applicationId, queue, new JsonMessageBodyConverter());
        }

        /// <summary>
        /// Обработка записей (Удаление статей)
        /// </summary>
        protected override void ProcessRecord()
        {
            var result = rabbitClient.SendRequest<RabbitResult<Article>>("article-delete", new Article { Id = ArticleId });
            string message = result.Success
                ? $"Article with identity \"{ArticleId}\" was successfully deleted"
                : $"Error occured while adding article: {result.Message}";
            Console.WriteLine(message);
        }

        /// <summary>
        /// Завершение обработки записей
        /// </summary>
        protected override void EndProcessing()
        {
            Console.WriteLine("Operation completed");
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            rabbitClient?.Dispose();
        }
    }
}
