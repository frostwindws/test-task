using System;
using System.Threading;
using System.Threading.Tasks;

namespace Articles.Messaging
{
    /// <summary>
    /// Интерфейс сервиса прослушивания сообщений
    /// </summary>
    public interface IListenerService
    {
        /// <summary>
        /// Запуск прослушивания сообщений
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены прослушивания</param>
        /// <returns>Задача выполнения прослушивания</returns>
        Task StartListenning(CancellationToken cancellationToken);
    }
}