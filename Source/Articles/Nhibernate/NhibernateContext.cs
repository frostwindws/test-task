using Articles.Models;
using NHibernate;

namespace Articles.Nhibernate
{
    /// <summary>
    /// Контекст работы с БД через NHibernate.
    /// </summary>
    public class NhibernateContext : IDataContext
    {
        private readonly ISession session;

        /// <summary>
        /// Репозиторий статей
        /// </summary>
        public IArticlesRepository Articles { get; }

        /// <summary>
        /// Репозиторий комментариев
        /// </summary>
        public ICommentsRepository Comments { get; }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <returns>Задача по сохранению изменений</returns>
        public void Commit()
        {
            session.Transaction.Commit();
        }

        /// <summary>
        /// Отмена изменений
        /// </summary>
        /// <returns>Задача по отмене изменений</returns>
        public void Rollback()
        {
            session.Transaction.Rollback();
        }

        /// <summary>
        /// Конструктор контекста.
        /// </summary>
        /// <param name="session">Используемая.</param>
        public NhibernateContext(ISession session)
        {
            this.session = session;
            session.BeginTransaction();
            Articles = new NhibernateArticlesRepository(session);
            Comments = new NhibernateCommentsRepository(session);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            session?.Dispose();
        }
    }
}