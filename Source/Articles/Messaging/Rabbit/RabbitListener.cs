using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Articles.Messaging.Rabbit
{
    /// <summary>
    /// Класс, обеспечивающий работу с шиной RabbitMQ.
    /// </summary>
    public class RabbitListener : IRequestListener
    {
        /// <summary>
        /// Максимальное количество попыток переподключений подряд.
        /// </summary>
        private const short ConnectionRetryMaxCount = 10;

        /// <summary>
        /// Таймаут для попыток переподключений.
        /// </summary>
        private readonly TimeSpan connectionRetryTimeout = TimeSpan.FromSeconds(10);

        private readonly IConnection connection;
        private IModel channel;
        private readonly string exchange;
        private readonly string listenningQueue;

        /// <summary>
        /// Количество текущих попыток переподключений подряд.
        /// </summary>
        private short connectionRetryCount;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="connection">Соединение для подключения к Rabbit.</param>
        /// <param name="exchange">Имя обмена для рассылки оповещений.</param>
        /// <param name="listenningQueue">Имя используемой для ожидания сообщений очереди.</param>
        public RabbitListener(IConnection connection, string exchange, string listenningQueue)
        {
            this.connection = connection;
            this.exchange = exchange;
            this.listenningQueue = listenningQueue;
            InitChanel();
        }

        /// <summary>
        /// Инициализация канала и очередей
        /// </summary>
        private void InitChanel()
        {
            channel = connection.CreateModel();
            channel.QueueDeclare(listenningQueue, durable: false, exclusive: false, autoDelete: false);
            channel.BasicQos(0, 1, false);
        }

        /// <summary>
        /// Запуск задачи прослушивания.
        /// </summary>
        /// <param name="onAcceptMessage">Обработчик принятия сообщения</param>
        /// <param name="cancellationToken">Маркер отмены прослушивания.</param>
        /// <returns>Задача прослушивания очереди сообщений.</returns>
        public async Task Listen(Action<Message> onAcceptMessage, CancellationToken cancellationToken)
        {
            try
            {
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: listenningQueue, autoAck: false, consumer: consumer);
                consumer.Received += (sender, args) =>
                {

                    Log.Information("Rabbit Listener: executing {Type} command", args.BasicProperties.Type);
                    onAcceptMessage(new Message
                    {
                        Id = args.BasicProperties.CorrelationId,
                        Type = args.BasicProperties.Type,
                        ReplyTo = args.BasicProperties.ReplyTo,
                        ApplicationId = args.BasicProperties.AppId,
                        Body = args.Body
                    });

                    ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                };

                Log.Information("RabbitMQ Listener has successfully started. Listenning for {queue} queue", listenningQueue);
                connectionRetryCount = 0;

                // Ожидание отмены прослушивания
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    Log.Information("RabbitMQ Listener operation was canceled ({queue} queue)", listenningQueue);
                    throw;
                }

                // Если операция не была отменена - производится попытка восстановить соединение
                Log.Error(e, "RabbitMQ Listener error occured ({queue} queue)", listenningQueue);
                if (++connectionRetryCount > ConnectionRetryMaxCount)
                {
                    Log.Information("RabbitMQ Listener has tryed to reconnect more then {ConnectionRetryMaxCount} to {queue} queue. Reconnection is aborted", ConnectionRetryMaxCount, listenningQueue);
                    throw;
                }

                await Task.Delay(connectionRetryTimeout, cancellationToken);
                Log.Error(e, "RabbitMQ Listener is trying to reconnect to {queue} queue ({connectionRetryCount})", listenningQueue, connectionRetryCount);
                await Listen(onAcceptMessage, cancellationToken);
            }
        }

        /// <summary>
        /// Отправка оповещения в очередь ожидания ответа.
        /// </summary>
        /// <param name="replyTo">Адреса ответа.</param>
        /// <param name="message">Передаваемое сообщение.</param>
        /// <returns>Задача отправки сообщения.</returns>
        public void Reply(string replyTo, Message message)
        {
            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Type = message.Type ?? properties.Type;
            properties.CorrelationId = message.Id;
            channel.BasicPublish("", replyTo, properties, message.Body);
        }

        /// <summary>
        /// Массовое оповещение через Exchange.
        /// </summary>
        /// <param name="message">Передаваемое сообщение.</param>
        public void Announce(Message message)
        {
            channel.ExchangeDeclare(exchange, "fanout");
            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Type = message.Type ?? properties.Type;
            properties.Persistent = false;
            properties.CorrelationId = message.Id;
            properties.AppId = message.ApplicationId;
            channel.BasicPublish(exchange, "", properties, message.Body);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}