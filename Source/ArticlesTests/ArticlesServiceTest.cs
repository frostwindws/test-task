using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Articles.Models;
using Articles.Services;
using Articles.Services.Models;
using AutoMapper;
using Moq;
using NUnit.Framework;
using MapperConfiguration = Articles.Initialization.MapperConfiguration;

namespace ArticlesTests
{
    /// <summary>
    /// Класс теста сервиса статей
    /// </summary>
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ArticlesServiceTest
    {
        private Mock<IDataContext> mock;
        private IArticlesService service;

        /// <summary>
        /// Конструктор теста сервиса статей
        /// </summary>
        public ArticlesServiceTest()
        {
            Mapper.Reset();
            MapperConfiguration.Init();
        }

        /// <summary>
        /// Инициализация тестов
        /// </summary>
        [SetUp]
        public void TestSetUp()
        {
            mock = new Mock<IDataContext>();
            service = new ArticlesService(mock.Object);
        }

        /// <summary>
        /// Сервис возвращает то же количество записей, что и репозиторий
        /// </summary>
        [Test]
        public void GetAll_RequestRecords_ReturnsRepositorySameLengthResult()
        {
            IEnumerable<Article> collection = Enumerable.Range(1, 10).Select(i => new Article
            {
                Id = i,
                Author = $"Test Author {i}",
                Title = $"Test Title {i}",
                Content = $"Test Content {i}",
                Created = DateTime.Now.AddHours(-i)
            });

            mock.Setup(a => a.Articles.GetCollection()).Returns(collection);

            IEnumerable<ArticleDto> result = service.GetAll();

            Assert.AreEqual(collection.Count(), result.Count());
        }

        /// <summary>
        /// Запрос имеющейся записи возвращает не пустой результат
        /// </summary>
        [Test]
        public void Get_RequestExistingRecord_ReturnsNotNull()
        {
            long searchId = 1;
            mock.Setup(a => a.Articles.Get(searchId)).Returns(new Article());
            mock.Setup(a => a.Comments.GetForArticle(searchId)).Returns(new Comment[0]);

            ArticleDto result = service.Get(searchId);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Запрос отсутствующей записи возвращает null 
        /// </summary>
        [Test]
        public void Get_RequestNotExistingRecord_ReturnsNull()
        {
            const long SearchId = 1;
            mock.Setup(a => a.Articles.Get(SearchId)).Returns((Article)null);

            ArticleDto result = service.Get(SearchId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Запрос создания вызывает метод создания репозитория 1 раз
        /// </summary>
        [Test]
        public void Create_RequestNewArticleCreating_RunsCreateMethodOnce()
        {
            var articleToCreate = new ArticleDto();
            var article = new Article();
            mock.Setup(a => a.Articles.Create(It.IsAny<Article>())).Returns(article);

            service.Create(articleToCreate);

            mock.Verify(r => r.Articles.Create(It.IsAny<Article>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewArticleCreate_ReturnsRecordWithCreatedId()
        {
            const long CreatedId = 1;
            var articleToCreate = new ArticleDto();
            var article = new Article { Id = CreatedId };
            mock.Setup(a => a.Articles.Create(It.IsAny<Article>())).Returns(article);

            ArticleDto createdArticle = service.Create(articleToCreate);

            Assert.AreEqual(createdArticle.Id, article.Id);
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void Update_RequestArticleUpdate_RunsRepositoryUpdateMethodOnce()
        {
            var articleToUpdate = new ArticleDto();
            mock.Setup(a => a.Articles.Update(It.IsAny<Article>()));

            service.Update(articleToUpdate);

            mock.Verify(r => r.Articles.Update(It.IsAny<Article>()), Times.Once);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void Delete_RequestArticleDelete_RunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            mock.Setup(m => m.Articles.Delete(TargetId));

            service.Delete(TargetId);

            mock.Verify(r => r.Articles.Delete(TargetId), Times.Once);
        }
    }
}
