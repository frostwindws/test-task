using System;
using System.Collections.Generic;
using Articles.Models;
using Articles.Services.Models;
using AutoMapper;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы со статьями
    /// </summary>
    public class ArticlesService : IArticlesService, IDisposable
    {
        /// <summary>
        /// Контекст работы с данными
        /// </summary>
        private IDataContext Context { get; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="context">Используемый контекст</param>
        public ArticlesService(IDataContext context)
        {
            Context = context;
        }
        
        /// <inheritdoc />
        public ArticleData[] GetAll()
        {
            IEnumerable<Article> articles = Context.Articles.GetCollection();
            return Mapper.Map<ArticleData[]>(articles);
        }

        /// <inheritdoc />
        public ArticleData Get(long id)
        {
            Article article = Context.Articles.Find(id);
            ArticleData data = Mapper.Map<ArticleData>(article);
            if (article != null)
            {
                IEnumerable<Comment> comments = Context.Comments.GetForArticle(article.Id);
                data.Comments = Mapper.Map<CommentData[]>(comments);
            }

            return data;
        }

        /// <inheritdoc />
        public long? Create(ArticleData article)
        {
            if (article != null)
            {
                Article dbArticle = Mapper.Map<Article>(article);
                return Context.Articles.Create(dbArticle);
            }

            return null;
        }

        /// <inheritdoc />
        public void Update(ArticleData article)
        {
            if (article != null)
            {
                Article dbArticle = Mapper.Map<Article>(article);
                Context.Articles.Update(dbArticle);
            }
        }

        /// <inheritdoc />
        public void Delete(long id)
        {
            Context.Articles.Delete(id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Context is IDisposable disposableContext)
            {
                disposableContext.Dispose();
            }
        }
    }
}
