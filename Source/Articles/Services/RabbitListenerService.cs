using System.Threading;
using System.Threading.Tasks;
using Articles.Messaging;
using Articles.Services.Executors;
using Serilog;

namespace Articles.Services
{
    /// <summary>
    /// Сервис прослушивания сообщений.
    /// </summary>
    public class RabbitListenerService : IListenerService
    {
        private readonly ListenersFactory listenersFactory;
        private readonly IExecutorsProvider executorsProvider;

        /// <summary>
        /// Конструктор сервиса прослушивания сообщений.
        /// </summary>
        /// <param name="listenersFactory">Фабрика прослушивателей.</param>
        /// <param name="executorsProvider">Провайдер для исполнителей команд.</param>
        public RabbitListenerService(ListenersFactory listenersFactory, IExecutorsProvider executorsProvider)
        {
            this.listenersFactory = listenersFactory;
            this.executorsProvider = executorsProvider;
        }

        /// <summary>
        /// Запуск прослушивания сообщений.
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены действия.</param>
        /// <returns></returns>
        public async Task StartListenning(CancellationToken cancellationToken)
        {
            using (var listener = listenersFactory.GetListener())
            {
                listener.AcceptMessage += ListenerOnAcceptMessage;
                await listener.Listen(cancellationToken);
            }
        }

        /// <summary>
        /// Обработчик события получения комманды.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="message">Полученное сообщение.</param>
        private void ListenerOnAcceptMessage(object sender, Message message)
        {
            ICommandExecutor executor = executorsProvider.Get(message.Type);
            if (executor != null)
            {
                ExecuteCommand((IRequestListener)sender, executor, message);
            }
            else
            {
                Log.Error("Received unknown command type message {Type}", message.Type);
            }
        }

        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="listener">Слушатель запросов.</param>
        /// <param name="executor">Экземпляр исполнителя.</param>
        /// <param name="message">Сообщение запроса.</param>
        /// <returns></returns>
        private void ExecuteCommand(IRequestListener listener, ICommandExecutor executor, Message message)
        {
            byte[] result = executor.Execute(message.Body);
            if (result != null)
            {
                var outgoing = new Message
                {
                    Id = message.Id,
                    Type = "articles_update",
                    Body = result
                };

                listener.Reply(message.ReplyTo, outgoing);
            }

        }
    }
}