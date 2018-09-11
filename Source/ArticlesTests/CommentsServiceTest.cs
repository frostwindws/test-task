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

            ResultDto<CommentDto[]> result = service.GetForArticle(ArticleId);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(collection.Count(), result.Data.Count());
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

            ResultDto<CommentDto[]> result = service.GetAll();

            Assert.IsTrue(result.Success);
            Assert.AreEqual(collection.Count(), result.Data.Count());
        }

        /// <summary>
        /// Запрос имеющейся записи возвращает не пустой результат
        /// </summary>
        [Test]
        public void Get_RequestExistingRecord_ReturnsNotNull()
        {
            long searchId = 1;
            mock.Setup(a => a.Comments.Get(searchId)).Returns(new Comment());

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
            mock.Setup(a => a.Comments.Get(SearchId)).Returns((Comment)null);

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
            var commentToCreate = new CommentDto
            {
                ArticleId = 1,
                Author = "author",
                Content = "content"
            };
            var comment = new Comment();
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(comment);

            ResultDto<CommentDto> result = service.Create(commentToCreate);

            Assert.IsTrue(result.Success);
            mock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewValidCommentCreate_ReturnsCreatedRecordId()
        {
            const long CreatedId = 1;
            var commentToCreate = new CommentDto
            {
                ArticleId = 1,
                Author = "author",
                Content = "content"
            };
            var comment = new Comment { Id = CreatedId };
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(comment);

            ResultDto<CommentDto> result = service.Create(commentToCreate);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Data.Id, comment.Id);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Create_RequestNewInvalidCommentCreate_NererRunsRepositoryCreateMethod()
        {
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(new Comment());
            var invalidComments = new CommentDto[]
            {
                new CommentDto { ArticleId = 0, Author = "author", Content = "content" },
                new CommentDto { ArticleId = 1, Author = null, Content = "content" },
                new CommentDto { ArticleId = 1, Author = "author", Content = null },
            };

            foreach (CommentDto commentToCreate in invalidComments)
            {
                ResultDto<CommentDto> result = service.Create(commentToCreate);

                Assert.IsFalse(result.Success);
            }

            mock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Never);
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void Update_RequestCommentValidUpdate_RunsRepositoryUpdateMethodOnce()
        {
            var commentToUpdate = new CommentDto
            {
                ArticleId = 1,
                Author = "author",
                Content = "content"
            };
            mock.Setup(a => a.Comments.Update(It.IsAny<Comment>()));

            ResultDto<CommentDto> result = service.Update(commentToUpdate);

            Assert.IsTrue(result.Success);
            mock.Verify(r => r.Comments.Update(It.IsAny<Comment>()), Times.Once);
        }

        /// <summary>
        /// Запрос создания возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void Update_RequestCommentValidUpdate_NererRunsRepositoryUpdateMethod()
        {
            mock.Setup(a => a.Comments.Create(It.IsAny<Comment>())).Returns(new Comment());
            var invalidComments = new CommentDto[]
            {
                new CommentDto { ArticleId = 0, Author = "author", Content = "content" },
                new CommentDto { ArticleId = 1, Author = null, Content = "content" },
                new CommentDto { ArticleId = 1, Author = "author", Content = null },
            };

            foreach (CommentDto commentToUpdate in invalidComments)
            {
                ResultDto<CommentDto> result = service.Update(commentToUpdate);

                Assert.IsFalse(result.Success);
            }

            mock.Verify(r => r.Comments.Create(It.IsAny<Comment>()), Times.Never);
        }

        /// <summary>
        /// Запрос удаления записи выполняет метод удаления репозитория 1 раз
        /// </summary>
        [Test]
        public void Delete_RequestNewCommentDelete_RunsRepositoryDeleteMethodOnce()
        {
            const long TargetId = 1;
            mock.Setup(m => m.Comments.Delete(TargetId));

            ResultDto<CommentDto> result = service.Delete(TargetId);

            Assert.IsTrue(result.Success);
            mock.Verify(r => r.Comments.Delete(TargetId), Times.Once);
        }
    }
}
