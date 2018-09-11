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
    /// Тест сервиса коментариев 
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CommentsServiceTest
    {
        private Mock<IDataContext> contextMock;
        private Mock<IModelValidator<Comment>> validatorMock;
        private ICommentsService service;

        /// <summary>
        /// Конструктор теста сервиса коментариев
        /// </summary>
        public CommentsServiceTest()
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
            validatorMock = new Mock<IModelValidator<Comment>>();
            service = new CommentsService(contextMock.Object, validatorMock.Object);
        }

        /// <summary>
        /// По запросу комментариев к статье сервис возвращает то же количество записей, что и репозиторий
        /// </summary>
        [Test]
        public void GetForArticle_RequestCommentsForArticle_ReturnsRepositorySameLengthResult()
        {
            const long ArticleId = 1;
            IEnumerable<Comment> collection = Enumerable.Range(1, 10).Select(i => new Comment
            {
                Id = i,
                ArticleId = ArticleId,
                Author = $"Test Author {i}",
                Content = $"Test Comment {i}",
                Created = DateTime.Now.AddHours(-i)
            });

            contextMock.Setup(a => a.Comments.GetForArticle(ArticleId)).Returns(collection);

            ResultDto<CommentDto[]> result = service.GetForArticle(ArticleId);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(collection.Count(), result.Data.Length);
        }

        /// <summary>
        /// Сервис возвращает то же количество записей, что и репозиторий
        /// </summary>
        [Test]
        public void GetAll_RequestRecords_ReturnsRepositorySameLengthResult()
        {
            IEnumerable<Comment> collection = Enumerable.Range(1, 10).Select(i => new Comment
            {
                Id = i,
                ArticleId = i,
                Author = $"Test Author {i}",
                Content = $"Test Comment {i}",
                Created = DateTime.Now.AddHours(-i)
            });

            contextMock.Setup(a => a.Comments.GetCollection()).Returns(collection);

            ResultDto<CommentDto[]> result = service.GetAll();

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
            contextMock.Setup(a => a.Comments.Get(searchId)).Returns(new Comment());

            ResultDto<CommentDto> result = service.Get(searchId);

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
            contextMock.Setup(a => a.Comments.Get(SearchId)).Returns((Comment)null);

            ResultDto<CommentDto> result = service.Get(SearchId);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
        }

        /// <summary>
        /// Запрос создания вызывает метод создания репозитория 1 раз
        /// </summary>
        [Test]
        public void Create_RequestNewValidCommentCreate_RunsRepositoryCreateMethodOnce()
        {
            contextMock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(new Comment());
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Comment>>(), It.IsAny<Comment>())).Returns(new string[0]);

            ResultDto<CommentDto> result = service.Create(new CommentDto());

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewValidCommentCreate_ReturnsCreatedRecordId()
        {
            const long CreatedId = 1;
            var comment = new Comment { Id = CreatedId };
            contextMock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(comment);
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Comment>>(), It.IsAny<Comment>())).Returns(new string[0]);

            ResultDto<CommentDto> result = service.Create(new CommentDto());

            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Data.Id, comment.Id);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewInvalidCommentCreate_NererRunsRepositoryCreateMethod()
        {
            contextMock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(new Comment());
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Comment>>(), It.IsAny<Comment>())).Returns(new[] { "invalid" });

            ResultDto<CommentDto> result = service.Create(new CommentDto());

            Assert.IsFalse(result.Success);
            contextMock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Never);
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void Update_RequestCommentValidUpdate_RunsRepositoryUpdateMethodOnce()
        {
            contextMock.Setup(a => a.Comments.Update(It.IsAny<Comment>()));
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Comment>>(), It.IsAny<Comment>())).Returns(new string[0]);

            ResultDto<CommentDto> result = service.Update(new CommentDto());

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Comments.Update(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Update_RequestCommentValidUpdate_NeverRunsRepositoryUpdateMethod()
        {
            contextMock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(new Comment());
            validatorMock.Setup(v => v.GetErrors(It.IsAny<IRepository<Comment>>(), It.IsAny<Comment>())).Returns(new[] { "invalid" });

            ResultDto<CommentDto> result = service.Update(new CommentDto());

            Assert.IsFalse(result.Success);
            contextMock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Never);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void Delete_RequestNewCommentDelete_RunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            contextMock.Setup(m => m.Comments.Delete(TargetId));

            ResultDto<CommentDto> result = service.Delete(TargetId);

            Assert.IsTrue(result.Success);
            contextMock.Verify(r => r.Comments.Delete(TargetId), Times.Once);
        }
    }
}
