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
        private Mock<IDataContext> mock;
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
            mock = new Mock<IDataContext>();
            service = new CommentsService(mock.Object);
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

            mock.Setup(a => a.Comments.GetForArticle(ArticleId)).Returns(collection);

            IEnumerable<CommentDto> result = service.GetForArticle(ArticleId);

            Assert.AreEqual(collection.Count(), result.Count());
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

            mock.Setup(a => a.Comments.GetCollection()).Returns(collection);

            IEnumerable<CommentDto> result = service.GetAll();

            Assert.AreEqual(collection.Count(), result.Count());
        }

        /// <summary>
        /// Запрос имеющейся записи возвращает не пустой результат
        /// </summary>
        [Test]
        public void Get_RequestExistingRecord_ReturnsNotNull()
        {
            long searchId = 1;
            mock.Setup(a => a.Comments.Get(searchId)).Returns(new Comment());

            CommentDto result = service.Get(searchId);

            Assert.IsNotNull(result);
        }

        /// <summary>
        /// Запрос отсутствующей записи возвращает null 
        /// </summary>
        [Test]
        public void Get_RequestNotExistingRecord_ReturnsNull()
        {
            const long SearchId = 1;
            mock.Setup(a => a.Comments.Get(SearchId)).Returns((Comment)null);

            CommentDto result = service.Get(SearchId);

            Assert.IsNull(result);
        }

        /// <summary>
        /// Запрос создания вызывает метод создания репозитория 1 раз
        /// </summary>
        [Test]
        public void Create_RequestNewCommentCreate_RunCreateMethodOnce()
        {
            var commentToCreate = new CommentDto();
            var comment = new Comment();
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(comment);

            service.Create(commentToCreate);

            mock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewCommentCreate_ReturnsCreatedRecordId()
        {
            const long CreatedId = 1;
            var commentToCreate = new CommentDto();
            var comment = new Comment { Id = CreatedId };
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(comment);

            CommentDto createdComment = service.Create(commentToCreate);

            Assert.AreEqual(createdComment.Id, comment.Id);
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void Update_RequestCommentUpdate_RunsRepositoryUpdateMethodOnce()
        {
            var commentToUpdate = new CommentDto();
            mock.Setup(a => a.Comments.Update(It.IsAny<Comment>()));

            service.Update(commentToUpdate);

            mock.Verify(r => r.Comments.Update(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void Delete_RequestNewCommentDelete_RunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            mock.Setup(m => m.Comments.Delete(TargetId));

            service.Delete(TargetId);

            mock.Verify(r => r.Comments.Delete(TargetId), Times.Once);
        }
    }
}
