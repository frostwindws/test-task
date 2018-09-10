using System;
using System.Collections.Generic;
using System.Linq;
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
        public ArticleDto[] GetAll()
        {
            IEnumerable<Article> articles = Context.Articles.GetCollection().OrderByDescending(a => a.Created);
            return Mapper.Map<ArticleDto[]>(articles);
        }

        /// <inheritdoc />
        public ArticleDto Get(long id)
        {
            Article article = Context.Articles.Get(id);
            ArticleDto data = Mapper.Map<ArticleDto>(article);
            if (article != null)
            {
                IEnumerable<Comment> comments = Context.Comments.GetForArticle(article.Id);
                data.Comments = Mapper.Map<CommentDto[]>(comments);
            }

            return data;
        }

        /// <inheritdoc />
        public ArticleDto Create(ArticleDto article)
        {
            if (article != null)
            {
                Article dbArticle = Mapper.Map<Article>(article);
                return Mapper.Map<ArticleDto>(Context.Articles.Create(dbArticle));
            }

            return null;
        }

        /// <inheritdoc />
        public ArticleDto Update(ArticleDto article)
        {
            if (article != null)
            {
                Article dbArticle = Mapper.Map<Article>(article);
                return Mapper.Map<ArticleDto>(Context.Articles.Update(dbArticle));
            }

            return null;
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
