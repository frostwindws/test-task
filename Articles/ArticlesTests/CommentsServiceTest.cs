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
    /// Тест сервиса коментариев 
    /// </summary>
    public class CommentsServiceTest
    {
        private readonly Mock<ICommentsRepository> mock;
        private readonly ICommentsService service;

        /// <summary>
        /// Конструктор теста сервиса коментариев
        /// </summary>
        public CommentsServiceTest()
        {
            mock = new Mock<ICommentsRepository>();
            service = new CommentsService(mock.Object);
        }

        /// <summary>
        /// По запросу комментариев к статье сервис возвращает то же количество записей, что и репозиторий
        /// </summary>
        [Test]
        public void GetForArticleReturnsRepositorySameLengthResult()
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

            mock.Setup(a => a.GetForArticle(ArticleId)).Returns(collection);
            IEnumerable<Comment> result = service.GetForArticle(ArticleId);

            Assert.AreEqual(collection.Count(), result.Count());
        }

        /// <summary>
        /// Запрос создания вызывает метод создания репозитория 1 раз и возвращает идентификатор созаданной записи
        /// </summary>
        [Test]
        public void CreateRunCreateMethodOnceReturnsCreatedRecordId()
        {
            const long CreatedId = 1;
            var commentToCreate = new Comment();
            mock.Setup(a => a.Create(commentToCreate)).Returns(CreatedId);
            long newId = service.Create(commentToCreate);

            mock.Verify(r => r.Create(commentToCreate), Times.Once, "Repository creation method wasn't called or was called more than once");
            Assert.AreEqual(newId, CreatedId, 0, "Returned identity isn't equal created identity");
        }

        /// <summary>
        /// Запрос обновления записи выполняет метод обновления записи репозитория 1 раз
        /// </summary>
        [Test]
        public void UpdateRunsRepositoryUpdateMethodOnce()
        {
            var commentToUpdate = new Comment();
            service.Update(commentToUpdate);

            mock.Verify(r => r.Update(commentToUpdate), Times.Once);
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
