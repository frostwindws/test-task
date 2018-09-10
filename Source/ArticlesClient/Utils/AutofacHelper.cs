using System;
using ArticlesClient.Clients;
using ArticlesClient.Clients.Wcf;
using Autofac;

namespace ArticlesClient.Utils
{
    /// <summary>
    /// Хэлпер, позволяющий формировать экземпляры классов для используемых интерфейсов
    /// </summary>
    public static class AutofacHelper
    {
        private static readonly IContainer container;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        static AutofacHelper()
        {
            container = BuildContainer();
        }

        /// <summary>
        /// Построение контейнера Autofac
        /// </summary>
        /// <returns>Сформированный контейнер</returns>
        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<WcfDataClient>().As<IDataClient>();
            return builder.Build();
        }

        /// <summary>
        /// Получение экземпляра класса текущего интерфейса
        /// </summary>
        /// <typeparam name="T">Запрашиваемый интерфейс</typeparam>
        /// <returns>Экземпляр соответствующего класса</returns>
        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}
