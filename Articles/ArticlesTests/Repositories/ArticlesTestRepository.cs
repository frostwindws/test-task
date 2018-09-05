using System;
using System.Collections.Generic;
using System.Linq;
using Articles;
using Articles.Models;

namespace ArticlesTests.Repositories
{
    /// <summary>
    /// Тестовый репозиторий статей
    /// </summary>
    public class ArticlesTestRepository : IArticlesRepository
    {
        private readonly List<Article> articles;

        /// <summary>
        /// Конструктор тестового репозитория
        /// </summary>
        public ArticlesTestRepository()
        {
            articles = Enumerable.Range(1, 10).Select(i => new Article
            {
                Id = i,
                Author = $"Test Author {i}",
                Title = $"Test Title {i}",
                Content = $"Test Content {i}",
                Created = DateTime.Now.AddHours(-i)
            }).ToList();
        }

        /// <inheritdoc />
        public IEnumerable<Article> GetCollection()
        {
            return articles;
        }

        /// <inheritdoc />
        public Article Find(long id)
        {
            return articles.FirstOrDefault(a => a.Id == id);
        }

        /// <inheritdoc />
        public long Create(Article record)
        {
            long newId = articles.Select(a => a.Id).DefaultIfEmpty().Max() + 1;
            record.Id = newId;
            articles.Add(record);
            return newId;
        }

        /// <inheritdoc />
        public void Update(Article record)
        {
            Article oldArticle = Find(record.Id);
            if (oldArticle != null)
            {
                oldArticle.Author = record.Author;
                oldArticle.Title = record.Title;
                oldArticle.Content = record.Content;
            }
        }

        /// <inheritdoc />
        public void Delete(long id)
        {
            articles.RemoveAll(a => a.Id == id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }
    }
}
