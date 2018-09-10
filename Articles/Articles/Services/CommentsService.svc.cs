using System;
using System.Collections.Generic;
using System.Linq;
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

        public CommentDto[] GetAll()
        {
            return Mapper.Map<CommentDto[]>(Context.Comments.GetCollection().OrderByDescending(a => a.Created));
        }

        public CommentDto Get(long id)
        {
            return Mapper.Map<CommentDto>(Context.Comments.Get(id));
        }

        /// <inheritdoc />
        public CommentDto[] GetForArticle(long articleid)
        {
            IEnumerable<Comment> comments = Context.Comments.GetForArticle(articleid).OrderByDescending(a => a.Created);
            return Mapper.Map<CommentDto[]>(comments);
        }

        /// <inheritdoc />
        public CommentDto Create(CommentDto comment)
        {
            if (comment != null)
            {
                Comment dbComment = Mapper.Map<Comment>(comment);
                return Mapper.Map<CommentDto>(Context.Comments.Create(dbComment));
            }

            return null;
        }

        /// <inheritdoc />
        public CommentDto Update(CommentDto comment)
        {
            if (comment != null)
            {
                Comment dbComment = Mapper.Map<Comment>(comment);
                return Mapper.Map<CommentDto>(Context.Comments.Update(dbComment));
            }

            return null;
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
