using System;
using System.Collections.Generic;
using System.Linq;
using Articles;
using Articles.Models;

namespace ArticlesTests.Repositories
{
    /// <summary>
    /// Тестовый репозиторий для комментариев
    /// </summary>
    public class CommentsTestRepository : ICommentsRepository
    {
        private readonly List<Comment> comments;

        /// <summary>
        /// Конструктор тестового репозитория
        /// </summary>
        public CommentsTestRepository()
        {
            comments = Enumerable.Range(1, 10).Select(i => new Comment
            {
                Id = i,
                ArticleId = 1,
                Author = $"Test Author {i}",
                Content = $"Test Content {i}",
                Created = DateTime.Now.AddHours(-i)
            }).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<Comment> GetCollection()
        {
            return comments;
        }

        /// <inheritdoc />
        public Comment Find(long id)
        {
            return comments.FirstOrDefault(c => c.Id == id);
        }

        /// <inheritdoc />
        public IEnumerable<Comment> GetForArticle(long articleId)
        {
            return comments.Where(c => c.ArticleId == articleId);
        }

        /// <inheritdoc />
        public long Create(Comment record)
        {
            long newId = comments.Select(a => a.Id).DefaultIfEmpty().Max() + 1;
            record.Id = newId;
            comments.Add(record);
            return newId;
        }

        /// <inheritdoc />
        public void Update(Comment record)
        {
            Comment oldComment = comments.FirstOrDefault(c => c.Id == record.Id);
            if (oldComment != null)
            {
                oldComment.Author = record.Author;
                oldComment.Content = record.Content;
            }
        }

        /// <inheritdoc />
        public void Delete(long id)
        {
            comments.RemoveAll(c => c.Id == id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
