using ArticlesShell.Connected_Services.CommentsService;
using ArticlesShell.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.ServiceModel;

namespace ArticlesShell.Cmdlets
{
    /// <summary>
    /// Командлет запроса комментариев к статье
    /// </summary>
    [Cmdlet(VerbsCommon.Get, nameof(Comment))]
    [OutputType(typeof(IEnumerable<Comment>))]
    public class GetComments : Cmdlet, IDisposable
    {
        /// <summary>
        /// Адрес конечной точки WCF по умолчанию
        /// </summary>
        private const string DefaultWcfUri = "http://localhost:51430/Services/CommentsService.svc";

        /// <summary>
        /// Клиент обращения к WCF сервису
        /// </summary>
        private CommentsServiceClient client;

        /// <summary>
        /// Параметр автора статьи
        /// </summary>
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Mandatory = true, HelpMessage = "Article identity")]
        public long ArticleId { get; set; }

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "WCF enpoint URI (default: \"" + DefaultWcfUri + "\")")]
        public string WcfUri { get; set; }

        /// <summary>
        /// Запуск обработки командлета
        /// </summary>
        protected override void BeginProcessing()
        {
            string endpoint = string.IsNullOrWhiteSpace(WcfUri) ? DefaultWcfUri : WcfUri;
            client = new CommentsServiceClient(new BasicHttpBinding(), new EndpointAddress(new Uri(endpoint)));
        }

        /// <summary>
        /// Обработка записи
        /// </summary>
        protected override void ProcessRecord()
        {
            IEnumerable<Comment> articleComments = client.GetForArticle(ArticleId).Data.Select(c => new Comment
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                Author = c.Author,
                Content = c.Content,
                Created = c.Created
            });

            WriteObject(articleComments, true);

        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)client)?.Dispose();
        }
    }
}
