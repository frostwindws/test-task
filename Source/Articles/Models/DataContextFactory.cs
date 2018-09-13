using System;

namespace Articles.Models
{
    /// <summary>
    /// Фабрика построения контекста.
    /// </summary>
    public class DataContextFactory
    {
        /// <summary>
        /// Функция построения контекста.
        /// </summary>
        private readonly Func<IDataContext> builder;

        /// <summary>
        /// Инициализация функции построения фабрики.
        /// </summary>
        /// <param name="contextBuilder">Используемая функция построения контекстов.</param>
        public DataContextFactory(Func<IDataContext> contextBuilder)
        {
            builder = contextBuilder;
        }

        /// <summary>
        /// Получить новый контекст.
        /// </summary>
        /// <returns>Возвращает сформированный фабрикой контекст.</returns>
        public IDataContext GetContext()
        {
            return builder?.Invoke();
        }
    }
}