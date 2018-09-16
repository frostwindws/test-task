using System;

namespace ArticlesService.Messaging
{
    /// <summary>
    /// Фабрика формирования слушателей запросов.
    /// </summary>
    public class ListenersFactory
    {
        /// <summary>
        /// Функция построения слушателей.
        /// </summary>
        private readonly Func<IRequestListener> builder;

        /// <summary>
        /// Инициализация функции построения фабрики.
        /// </summary>
        /// <param name="listenerBuilder">Используемая функция построения слушателей.</param>
        public ListenersFactory(Func<IRequestListener> listenerBuilder)
        {
            builder = listenerBuilder;
        }

        /// <summary>
        /// Получить новый слушатель.
        /// </summary>
        /// <returns>Возвращает сформированный фабрикой слушатель.</returns>
        public IRequestListener GetListener()
        {
            return builder?.Invoke();
        }
    }
}