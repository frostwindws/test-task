using System;
using ArticlesClient.Clients;

namespace ArticlesClient.Utils
{
    /// <summary>
    /// Фабрика клиентов.
    /// </summary>
    public class DataClientsFactory
    {
        /// <summary>
        /// Метод формирования клиента.
        /// </summary>
        private readonly Func<IDataClient> factoryMethod;

        /// <summary>
        /// Конструктор фабрики.
        /// </summary>
        /// <param name="method">Метод формирования клиентов.</param>
        public DataClientsFactory(Func<IDataClient> method)
        {
            factoryMethod = method;
        }

        /// <summary>
        /// Запрос на получение клиента.
        /// </summary>
        /// <returns></returns>
        public IDataClient GetClient()
        {
            return factoryMethod.Invoke();
        }
    }
}
