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
        /// Запуск задачи прослушивания.
        /// </summary>
        /// <param name="onAcceptMessage">Обработчик принятия сообщения</param>
        /// <param name="cancellationToken">Маркер отмены прослушивания.</param>
        /// <returns>Задача прослушивания очереди сообщений.</returns>
        Task Listen(Action<Message> onAcceptMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Отправка оповещения.
        /// </summary>
        /// <param name="replyTo">Адреса ответа.</param>
        /// <param name="message">Передаваемое сообщение.</param>
        /// <returns>Задача отправки сообщения.</returns>
        void Reply(string replyTo, Message message);

        /// <summary>
        /// Массовое оповещение через Exchange.
        /// </summary>
        /// <param name="message">Передаваемое сообщение.</param>
        void Announce(Message message);
    }
}
