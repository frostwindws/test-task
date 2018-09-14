using System;
using System.Management.Automation;
using ArticlesShell.Conververters;
using ArticlesShell.Models;
using ArticlesShell.Rabbit;
using RabbitMQ.Client;

namespace ArticlesShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "Article")]
    public class DeleteArticle : Cmdlet, IDisposable
    {
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
        /// Запуск обработки
        /// </summary>
        protected override void BeginProcessing()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            rabbitClient = new RabbitClient(factory.CreateConnection(), "articles-requests-queue", new JsonMessageBodyConverter());
        }

        /// <summary>
        /// Обработка записей (Добавление статей)
        /// </summary>
        protected override void ProcessRecord()
        {
            var result = rabbitClient.SendRequest<RabbitResult<Article>>("article-delete", new Article { Id = ArticleId });
            if (result.Success)
            {
                Console.WriteLine($"Article with identity \"{ArticleId}\" was successfully deleted");
            }
            else
            {
                Console.WriteLine($"Error occured while adding article: {result.Message}");
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
