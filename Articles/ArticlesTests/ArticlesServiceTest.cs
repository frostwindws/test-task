using System;
using System.Collections.Generic;
using System.Linq;
using Articles;
using Articles.Models;
using Articles.Services;
using ArticlesTests.Repositories;
using NUnit.Framework;

namespace ArticlesTests
{
    /// <summary>
    /// Класс теста сервиса статей
    /// </summary>
    [TestFixture]
    public class ArticlesServiceTest : ArticlesService
    {
        public ArticlesServiceTest()
            : base(null)
        {
        }

        [SetUp]
        public void TestsSetup()
        {
            Repository = new ArticlesTestRepository();
        }

        [Test]
        public void GetAllReturnsRepositorySameLengthResult()
        {
            IEnumerable<Article> result = GetAll();
            IEnumerable<Article> allRecords = Repository.GetCollection();

            Assert.AreEqual(result.Count(), allRecords.Count());
        }

        [Test]
        public void GetRequestExistingRecordReturnsNotNull()
        {
            Article existingArticle = Repository.GetCollection().First();
            Article result = Get(existingArticle.Id);

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetRequestNotExistingRecordReturnsNull()
        {
            Article result = Get(-1);

            Assert.IsNull(result);
        }

        [Test]
        public void CreateAddNewRecordReturnsExistingRecordId()
        {
            long newId = Create(new Article());
            Article addedArticle = Repository.Find(newId);

            Assert.IsNotNull(addedArticle);
        }

        [Test]
        public void UpdateExistingRecordPropertiesUpdatedExceptCreatedDate()
        {
            long existingArticleId = Repository.GetCollection().First().Id;
            const string Author = "Updated Author";
            const string Title = "Updated Title";
            const string Content = "Updated Content";
            DateTime created = DateTime.MinValue;

            Update(new Article
            {
                Id = existingArticleId,
                Author = Author,
                Title = Title,
                Content = Content,
                Created = created
            });

            Article article = Repository.Find(existingArticleId);

            Assert.AreEqual(article.Author, Author, "Автор статьи не был обновлен");
            Assert.AreEqual(article.Title, Title, "Заголовок статьи не был обновлен");
            Assert.AreEqual(article.Content, Content, "Контент статьи не был обновлен");
            Assert.AreNotEqual(article.Created, created, "Дата создания статьи была обновлен");
        }

        [Test]
        public void DeleteExistingRecordRecordIsNotInRepository()
        {
            Article article = Repository.GetCollection().First();

            Delete(article.Id);
            article = Repository.Find(article.Id);

            Assert.IsNull(article);
        }

        [Test]
        public void DeleteExistingRecordRecordRepositoryCountChangedByOne()
        {
            int initialCount = Repository.GetCollection().Count();
            Article article = Repository.GetCollection().First();

            Delete(article.Id);
            int countAfterDelete = Repository.GetCollection().Count();

            Assert.AreEqual(initialCount, countAfterDelete + 1);
        }

        [Test]
        public void DeleteNotExistingRecordRepositoryCountIsTheSame()
        {
            int initialCount = Repository.GetCollection().Count();

            Delete(-1);
            int countAfterDelete = Repository.GetCollection().Count();

            Assert.AreEqual(initialCount, countAfterDelete);
        }
    }
}
