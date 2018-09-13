using System;
using System.Threading;
using System.Threading.Tasks;

namespace Articles.Messaging
{
    /// <summary>
    /// Интерфейс слушателя запросов.
    /// </summary>
    public interface IRequestListener: IDisposable
    {
        /// <summary>
        /// Событие получения запроса.
        /// </summary>
        event EventHandler<Message> AcceptMessage;

        /// <summary>
        /// Запуск задачи прослушивания.
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены прослушивания.</param>
        /// <returns>Задача прослушивания очереди сообщений.</returns>
        Task Listen(CancellationToken cancellationToken);

        /// <summary>
        /// Отправка оповещения.
        /// </summary>
        /// <param name="replyTo">Адреса ответа.</param>
        /// <param name="message">Передаваемое сообщение.</param>
        /// <returns>Задача отправки сообщения.</returns>
        void Reply(string replyTo, Message message);
    }
}
