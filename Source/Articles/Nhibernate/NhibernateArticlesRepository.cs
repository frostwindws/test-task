using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Articles.Models;
using NHibernate;
using NHibernate.Criterion;

namespace Articles.Nhibernate
{
    /// <summary>
    /// Репозиторий статей на NHibernate
    /// </summary>
    public class NhibernateArticlesRepository : IArticlesRepository
    {
        private readonly ISession session;

        /// <summary>
        /// Конструктор репозитория
        /// </summary>
        /// <param name="session">Используемая сессия</param>
        public NhibernateArticlesRepository(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Получить список статей.
        /// </summary>
        /// <returns>Возвращает полный список статей.</returns>
        public IEnumerable<Article> GetCollection()
        {
            return session.Query<Article>().OrderByDescending(a => a.Created).ToArray();
        }

        /// <summary>
        /// Проверка наличия статьи с указанным свойством.
        /// </summary>
        /// <param name="propertyName">Имя свойства.</param>
        /// <param name="value">Значение свойства.</param>
        /// <returns>True, если статья с указанным свойством уже существует.</returns>
        public bool Exists(string propertyName, string value)
        {
            int count = session.CreateCriteria<Article>()
                    .Add(Restrictions.Eq(propertyName, value))
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();
            return count > 0;
        }

        /// <summary>
        /// Найти статью по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор искомой статьи.</param>
        /// <returns>
        /// Возвращает найденую статью.
        /// Если статья отсутствует - возвращает null.</returns>
        public Article Get(long id)
        {
            return session.Get<Article>(id);
        }

        /// <summary>
        /// Создать новую статью.
        /// </summary>
        /// <param name="record">Создаваемая статья.</param>
        /// <returns>Возвращает идентификатор созданной статьи.</returns>
        public Article Create(Article record)
        {
            session.Save(record);
            return record;
        }

        /// <summary>
        /// Обновление имеющейся статьи.
        /// </summary>
        /// <param name="record">Обновляемая статья.</param>
        public Article Update(Article record)
        {
            session.Update(record);
            return record;
        }

        /// <summary>
        /// Удалить статью.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи.</param>
        public void Delete(long id)
        {
            session.Query<Article>().Where(a => a.Id == id).Delete();
        }
    }
}