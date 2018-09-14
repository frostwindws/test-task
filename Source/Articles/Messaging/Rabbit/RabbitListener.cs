using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

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


        private readonly ConnectionFactory connectionFactory;
        private readonly string queue;
        private IConnection connection;

        /// <summary>
        /// Количество текущих попыток переподключений подряд.
        /// </summary>
        private short connectionRetryCount;

        /// <summary>
        /// Событие получения сообщения.
        /// </summary>
        public event EventHandler<Message> AcceptMessage;

        /// <summary>
        /// Используемое соединение.
        /// </summary>
        private IConnection Connection
        {
            get
            {
                if (connection == null || !connection.IsOpen)
                {
                    // Если соединение еще не получено или уже закрыто - производится его пересоздание
                    connection = connectionFactory.CreateConnection();
                }

                return connection;
            }
        }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="connectionFactory">Фабрика, используемая для создания соединений.</param>
        /// <param name="queue">Имя используемой очереди.</param>
        public RabbitListener(ConnectionFactory connectionFactory, string queue)
        {
            // передается фабрика, а не конечное соединение для обеспечения возможности слушателя 
            // при необходимости восстановить соединение при его обрыве.
            this.connectionFactory = connectionFactory;
            this.queue = queue;
        }

        /// <summary>
        /// Запуск задачи прослушивания.
        /// </summary>
        /// <param name="cancellationToken">Маркер отмены прослушивания.</param>
        /// <returns>Задача прослушивания очереди сообщений.</returns>
        public async Task Listen(CancellationToken cancellationToken)
        {
            try
            {
                using (var channel = Connection.CreateModel())
                {
                    channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false);
                    channel.BasicQos(0, 1, false);
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
                    consumer.Received += (sender, args) =>
                    {
                        try
                        {
                            Log.Information("Rabbit Listener: executing {Type} command", args.BasicProperties.Type);
                            AcceptMessage.Invoke(this,
                                new Message
                                {
                                    Id = args.BasicProperties.CorrelationId,
                                    Type = args.BasicProperties.Type,
                                    ReplyTo = args.BasicProperties.ReplyTo,
                                    Body = args.Body
                                });
                        }
                        finally
                        {
                            ((EventingBasicConsumer)sender).Model.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                        }
                    };

                    Log.Information("RabbitMQ Listener has successfully started. Listenning for {queue} queue", queue);
                    connectionRetryCount = 0;

                    // Ожидание отмены прослушивания
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    Log.Information("RabbitMQ Listener operation was canceled ({queue} queue)", queue);
                    throw;
                }

                // Если операция не была отменена - производится попытка восстановить соединение
                Log.Error(e, "RabbitMQ Listener error occured ({queue} queue)", queue);
                if (++connectionRetryCount > ConnectionRetryMaxCount)
                {
                    Log.Information("RabbitMQ Listener has tryed to reconnect more then {ConnectionRetryMaxCount} to {queue} queue. Reconnection is aborted", ConnectionRetryMaxCount, queue);
                    throw;
                }

                await Task.Delay(connectionRetryTimeout, cancellationToken);
                Log.Error(e, "RabbitMQ Listener is trying to reconnect to {queue} queue ({connectionRetryCount})", queue, connectionRetryCount);
                await Listen(cancellationToken);
            }
        }

        /// <summary>
        /// Отправка оповещения.
        /// </summary>
        /// <param name="replyTo">Адреса ответа.</param>
        /// <param name="message">Передаваемое сообщение.</param>
        /// <returns>Задача отправки сообщения.</returns>
        public void Reply(string replyTo, Message message)
        {
            using (var сhannel = Connection.CreateModel())
            {
                //сhannel.ExchangeDeclare("", "direct");
                IBasicProperties properties = сhannel.CreateBasicProperties();
                properties.Type = message.Type?? properties.Type;
                properties.CorrelationId = message.Id;
                сhannel.BasicPublish("", replyTo, properties, message.Body);
            }
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