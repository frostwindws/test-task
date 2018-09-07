using System;
using System.Collections.Generic;
using System.Linq;
using Articles.Models;
using Articles.Services;
using Moq;
using NUnit.Framework;

namespace ArticlesTests
{
    /// <summary>
    /// Класс теста сервиса статей
    /// </summary>
    [TestFixture]
    public class ArticlesServiceTest
    {
        private readonly Mock<IArticlesRepository> mock;
        private readonly IArticlesService service;

        /// <summary>
        /// Конструктор теста сервиса статей
        /// </summary>
        public ArticlesServiceTest()
        {
            mock = new Mock<IArticlesRepository>();
            service = new ArticlesService(mock.Object);
        }

        /// <summary>
        /// Сервис возвращает то же количество записей, что и репозиторий
        /// </summary>
        [Test]
        public void GetAllReturnsRepositorySameLengthResult()
        {
            IEnumerable<Article> collection = Enumerable.Range(1, 10).Select(i => new Article
            {
                Id = i,
                Author = $"Test Author {i}",
                Title = $"Test Title {i}",
                Content = $"Test Content {i}",
                Created = DateTime.Now.AddHours(-i)
            });

            mock.Setup(a => a.GetCollection()).Returns(collection);
            IEnumerable<Article> result = service.GetAll();

           Assert.AreEqual(collection.Count(), result.Count());
        }

        /// <summary>
        /// Запрос имеющейся записи возвращает не пустой результат
        /// </summary>
        [Test]
        public void GetRequestExistingRecordReturnsNotNull()
        {
            long searchId = 1;
            mock.Setup(a => a.Find(searchId)).Returns(new Article());
            Article result = service.Get(searchId);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Запрос отсутствующей записи возвращает null 
        /// </summary>
        [Test]
        public void GetRequestNotExistingRecordReturnsNull()
        {
            const long SearchId = 1;
            mock.Setup(a => a.Find(SearchId)).Returns((Article)null);
            Article result = service.Get(SearchId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Запрос создания вызывает метод создания репозитория 1 раз и возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void CreateRunCreateMethodOnceReturnsCreatedRecordId()
        {
            const long CreatedId = 1;
            var articleToCreate = new Article();
            mock.Setup(a => a.Create(articleToCreate)).Returns(CreatedId);
            long newId = service.Create(articleToCreate);

            mock.Verify(r => r.Create(articleToCreate), Times.Once, "Repository creation method wasn't called or was called more than once");
            Assert.AreEqual(newId, CreatedId, 0, "Returned identity isn't equal created identity");
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void UpdateRunsRepositoryUpdateMethodOnce()
        {
            var articleToUpdate = new Article();
            service.Update(articleToUpdate);

            mock.Verify(r => r.Update(articleToUpdate), Times.Once);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void DeleteRunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            service.Delete(TargetId);

            mock.Verify(r => r.Delete(TargetId), Times.Once);
        }
    }
}
