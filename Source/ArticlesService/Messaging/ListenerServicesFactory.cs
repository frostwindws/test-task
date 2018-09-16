using System;
using Autofac;

namespace ArticlesService.Messaging
{
    public class ListenerServicesFactory : IDisposable
    {
        /// <summary>
        /// Контейнер построения сервиса прослушивания.
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// Конструктор фабрики листенеров.
        /// </summary>
        /// <param name="container"></param>
        public ListenerServicesFactory(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Запрос на получение сервиса прослушивания сообщений.
        /// </summary>
        /// <returns></returns>
        public IListenerService GetListenerService()
        {
            return container.Resolve<IListenerService>();
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            container?.Dispose();
        }
    }
}