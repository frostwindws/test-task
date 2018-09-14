using System.Collections.Generic;
using System.Linq;
using Articles.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Articles.Nhibernate
{
    /// <summary>
    /// Репозиторий комментариев на NHibernate
    /// </summary>
    public class NhibernateCommentsRepository : ICommentsRepository
    {
        private readonly ISession session;

        /// <summary>
        /// Конструктор репозитория
        /// </summary>
        /// <param name="session">Используемая сессия</param>
        public NhibernateCommentsRepository(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Получить все имеющиеся комментарии.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Comment> GetCollection()
        {
            return session.Query<Comment>().OrderByDescending(a => a.Created).ToArray();
        }

        /// <summary>
        /// Получить список комментариев для статьи.
        /// </summary>
        /// <param name="articleId">Идентификатор статьи.</param>
        /// <returns>Возвращает список комментариев для указанной статьи.</returns>
        public IEnumerable<Comment> GetForArticle(long articleId)
        {
            return session.Query<Comment>()
                .Where(c => c.ArticleId == articleId)
                .ToArray();
        }

        /// <summary>
        /// Найти комментарий по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомого комментария.</param>
        /// <returns>
        /// Возвращает найденный комментарий.
        /// Если комментарий с указанным идентификатором отсутствует, возвращает null.
        /// </returns>
        public Comment Get(long id)
        {
            return session.Get<Comment>(id);
        }

        /// <summary>
        /// Проверка наличия комментария с указанным свойством.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <returns>True, если статья с указанным свойством уже существует.</returns>
        public bool Exists(string propertyName, string value)
        {
            int count = session.CreateCriteria<Comment>()
                .CreateCriteria(propertyName, value)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
            return count > 0;
        }

        /// <summary>
        /// Создать новый комментарий.
        /// </summary>
        /// <param name="comment">Создаваемый комментарий.</param>
        /// <returns>Возвращает идентификатор созданного комментария.</returns>
        public Comment Create(Comment comment)
        {
            session.Save(comment);
            return comment;
        }

        /// <summary>
        /// Обновить имеющийся комментарий.
        /// </summary>
        /// <param name="comment">Обновляемый комментарий.</param>
        public Comment Update(Comment comment)
        {
            session.Update(comment);
            return comment;
        }

        /// <summary>
        /// Удалить комментарий.
        /// </summary>
        /// <param name="id">Идентификатор комментария.</param>
        public void Delete(long id)
        {
            session.Query<Comment>().Where(a => a.Id == id).Delete();
        }
    }
}