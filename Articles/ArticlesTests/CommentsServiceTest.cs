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
    /// Тест сервиса коментариев 
    /// </summary>
    public class CommentsServiceTest : CommentsService
    {
        public CommentsServiceTest(): base(null)
        {
        }

        [SetUp]
        public void TestsSetup()
        {
            Repository = new CommentsTestRepository();
        }

        [Test]
        public void GetForArticleRequestExistingArticleCommentsReturnsRepositorySameLengthResult()
        {
            IEnumerable<Comment> requestedComments = Repository.GetForArticle(1);
            IEnumerable<Comment> result = GetForArticle(1);

            Assert.AreEqual(requestedComments.Count(), result.Count());
        }

        [Test]
        public void CreateAddNewRecordReturnsExistingRecordId()
        {
            long newId = Create(new Comment());
            Comment addedComment = Repository.Find(newId);

            Assert.IsNotNull(addedComment);
        }

        [Test]
        public void UpdateExistingRecordPropertiesUpdatedExceptCreatedDate()
        {
            long existingCommentId = Repository.GetCollection().First().Id;
            const string Author = "Updated Author";
            const int ArticleId = 5;
            const string Content = "Updated Content";
            DateTime created = DateTime.MinValue;

            Update(new Comment
            {
                Id = existingCommentId,
                ArticleId = ArticleId,
                Author = Author,
                Content = Content,
                Created = created
            });

            Comment comment = Repository.Find(existingCommentId);

            Assert.AreEqual(comment.Author, Author, "Автор статьи не был обновлен");
            Assert.AreEqual(comment.Content, Content, "Контент статьи не был обновлен");
            Assert.AreNotEqual(comment.ArticleId, ArticleId, "Идентификатор статьи был обновлен");
            Assert.AreNotEqual(comment.Created, created, "Дата создания статьи была обновлен");
        }

        [Test]
        public void DeleteExistingRecordRecordIsNotInRepository()
        {
            Comment comment = Repository.GetCollection().First();

            Delete(comment.Id);
            comment = Repository.Find(comment.Id);

            Assert.IsNull(comment);
        }

        [Test]
        public void DeleteExistingRecordRecordRepositoryCountChangedByOne()
        {
            int initialCount = Repository.GetCollection().Count();
            long existingCommentId = Repository.GetCollection().First().Id;

            Delete(existingCommentId);
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
