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
        private Mock<IDataContext> contextMock;
        private Mock<IModelValidator<Article>> validatorMock;
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
            contextMock = new Mock<IDataContext>();
            validatorMock = new Mock<IModelValidator<Article>>();
            service = new ArticlesService(contextMock.Object, validatorMock.Object);
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

            contextMock.Setup(a => a.Articles.GetCollection()).Returns(collection);

            ResultDto<ArticleDto[]> result = service.GetAll();

            Assert.IsTrue(result.Success);
            Assert.AreEqual(collection.Count(), result.Data.Length);
        }

        /// <summary>
        /// Запрос имеющейся записи возвращает не пустой результат
        /// </summary>
        [Test]
        public void Get_RequestExistingRecord_ReturnsNotNull()
        {
            long searchId = 1;
            contextMock.Setup(a => a.Articles.Get(searchId)).Returns(new Article());
            contextMock.Setup(a => a.Comments.GetForArticle(searchId)).Returns(new Comment[0]);

            ResultDto<ArticleDto> result = service.Get(searchId);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
        }

        /// <summary>
        /// Запрос отсутствующей записи возвращает null 
        /// </summary>
        [Test]
        public void Get_RequestNotExistingRecord_ReturnsNull()
        {
            const long SearchId = 1;
            contextMock.Setup(a => a.Articles.Get(SearchId)).Returns((Article)null);

            ResultDto<ArticleDto> result = service.Get(SearchId);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
        }

        /// <summary>
        /// Запрос создания корректной статьи вызывает метод создания репозитория 1 раз
        /// </summary>
        [Test]
        public void Create_RequestNewValidArticleCreating_RunsRepositoryCreateMethodOnce()
        {
            contextMock.Setup(a => a.Articles.Create(It.IsAny<Article>())).Returns(new Article());
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Article>>(), It.IsAny<Article>())).Returns(new string[0]);

            ResultDto<ArticleDto> result = service.Create(new ArticleDto());

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Articles.Create(It.IsAny<Article>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания некорректной статьи не вызывает метод создания репозитория
        /// </summary>
        [Test]
        public void Create_RequestNewInvalidArticleCreating_NeverRunsRepositoryCreateMethod()
        {
            contextMock.Setup(a => a.Articles.Create(It.IsAny<Article>())).Returns(new Article());
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Article>>(), It.IsAny<Article>())).Returns(new[] { "is invalid" });

            ResultDto<ArticleDto> result = service.Create(new ArticleDto());
            Assert.IsFalse(result.Success);

            contextMock.Verify(r => r.Articles.Create(It.IsAny<Article>()), Times.Never);
        }

        /// <summary>
        /// Запрос создания корректной статьи возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewValidArticleCreate_ReturnsRecordWithCreatedId()
        {
            var article = new Article { Id = 1 };
            contextMock.Setup(a => a.Articles.Create(It.IsAny<Article>())).Returns(article);
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Article>>(), It.IsAny<Article>())).Returns(new string[0]);

            ResultDto<ArticleDto> result = service.Create(new ArticleDto());

            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Data.Id, article.Id);
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void Update_RequestArticleValidUpdate_RunsRepositoryUpdateMethodOnce()
        {
            contextMock.Setup(a => a.Articles.Update(It.IsAny<Article>()));
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Article>>(), It.IsAny<Article>())).Returns(new string[0]);

            ResultDto<ArticleDto> result = service.Update(new ArticleDto());

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Articles.Update(It.IsAny<Article>()), Times.Once);
        }

        /// <summary>
        /// Запрос некорректного обновления записи не выполняет метод обновления записи репозитория
        /// </summary>
        [Test]
        public void Update_RequestArticleValidUpdate_NeverRunsRepositoryUpdateMethod()
        {
            contextMock.Setup(a => a.Articles.Update(It.IsAny<Article>()));
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Article>>(), It.IsAny<Article>())).Returns(new[] { "is invalid" });

            ResultDto<ArticleDto> result = service.Update(new ArticleDto());

            Assert.IsFalse(result.Success);
            contextMock.Verify(r => r.Articles.Update(It.IsAny<Article>()), Times.Never);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void Delete_RequestArticleDelete_RunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            contextMock.Setup(m => m.Articles.Delete(TargetId));

            ResultDto<ArticleDto> result = service.Delete(TargetId);

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Articles.Delete(TargetId), Times.Once);
        }
    }
}
