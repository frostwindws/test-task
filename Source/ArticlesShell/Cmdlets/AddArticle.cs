using System;
using System.IO;
using System.Management.Automation;
using ArticlesShell.Conververters;
using ArticlesShell.Models;
using ArticlesShell.Rabbit;
using RabbitMQ.Client;

namespace ArticlesShell.Cmdlets
{
    /// <summary>
    /// Командлет добавления новой статьи
    /// </summary>
    [Cmdlet(VerbsCommon.Add, nameof(Article))]
    public class AddArticle : Cmdlet, IDisposable
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
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Author of the article")]
        public string Author { get; set; }

        /// <summary>
        /// Параметр заголовка статьи
        /// </summary>
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Article")]
        public string Title { get; set; }

        /// <summary>
        /// Параметр флага чтения контента статьи из файла
        /// </summary>
        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = "Set to true to read content from file")]
        public bool FromFile { get; set; }

        /// <summary>
        /// Параметр контента статьи
        /// </summary>
        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = "Content of the article (path to content file if FromFile is set to true)")]
        public string Content { get; set; }

        /// <summary>
        /// Параметр адреса шины RabbitQM
        /// </summary>
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "RabbitQM Uri (default: \"" + DefaultRabbitUri + "\")")]
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
        /// Обработка записей (Добавление статей)
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (FromFile)
                {
                    Content = File.ReadAllText(Content);
                }

                var article = new Article
                {
                    Author = Author,
                    Title = Title,
                    Content = Content
                };

                var result = rabbitClient.SendRequest<RabbitResult<Article>>("article-create", article);
                if (result.Success)
                {
                    Console.WriteLine($"Article \"{result.Data.Title}\" by {result.Data.Author} was succesfully created at {result.Data.Created:dd.MM.yyyy HH:mm:ss}");
                }
                else
                {
                    Console.WriteLine($"Error occured while adding article: {result.Message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while adding article: {e.Message}");
                throw;
            }
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
