using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using ArticlesShell.Connected_Services.CommentsService;
using ArticlesShell.Models;

namespace ArticlesShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Comments")]
    [OutputType(typeof(IEnumerable<Comment>))]
    public class GetComments : Cmdlet, IDisposable
    {
        private CommentsServiceClient client;

        private static List<Comment> comments;

        /// <summary>
        /// Параметр автора статьи
        /// </summary>
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Mandatory = true, HelpMessage = "Article identity")]
        public long ArticleId { get; set; }

        protected override void BeginProcessing()
        {
            client = new CommentsServiceClient();
        }

        protected override void ProcessRecord()
        {
            if (comments == null)
            {
                comments = new List<Comment>();
            }

            comments.AddRange(client.GetForArticle(ArticleId).Data.Select(c => new Comment
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                Author = c.Author,
                Content = c.Content,
                Created = c.Created
            }));
        }

        public static IEnumerable<Comment> GetArticlesComments()
        {
            return comments;
        }

        public void Dispose()
        {
            ((IDisposable)client)?.Dispose();
        }
    }
}
