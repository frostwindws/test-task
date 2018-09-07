using System;
using System.Collections.Generic;
using Articles.Models;
using Articles.Services.Models;
using AutoMapper;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы с комментариями к статьям
    /// </summary>
    public class CommentsService : ICommentsService, IDisposable
    {
        /// <summary>
        /// Контекст работы с данными
        /// </summary>
        private IDataContext Context { get; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="context">Используемый контекст</param>
        public CommentsService(IDataContext context)
        {
            Context = context;
        }

        /// <inheritdoc />
        public CommentData[] GetForArticle(long articleid)
        {
            IEnumerable<Comment> comments = Context.Comments.GetForArticle(articleid);
            return Mapper.Map<CommentData[]>(comments);
        }

        /// <inheritdoc />
        public long? Create(CommentData comment)
        {
            if (comment != null)
            {
                Comment dbComment = Mapper.Map<Comment>(comment);
                return Context.Comments.Create(dbComment);
            }

            return null;
        }

        /// <inheritdoc />
        public void Update(CommentData comment)
        {
            if (comment != null)
            {
                Comment dbComment = Mapper.Map<Comment>(comment);
                Context.Comments.Update(dbComment);
            }
        }

        /// <inheritdoc />
        public void Delete(long id)
        {
            Context.Comments.Delete(id);
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
