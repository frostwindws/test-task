using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArticlesWeb.Clients.Rabbit
{
    /// <summary>
    /// Провайдер запросов к RabbitMQ.
    /// </summary>
    public class RabbitRequestProvider : IDisposable
    {
        private readonly IConnection connection;
        private readonly string applicationId;
        private readonly IModel channel;
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();

        /// <summary>
        /// Конструктор запроса.
        /// </summary>
        /// <param name="connection">Используемое соединение.</param>
        /// <param name="applicationId">Идентификатор сообщений (необходим для отбрасывания оповещений событий, источником которых было само приложение)</param>
        public RabbitRequestProvider(IConnection connection, string applicationId)
        {
            this.connection = connection;
            this.applicationId = applicationId;
            channel = connection.CreateModel();
        }

        /// <summary>
        /// Отправка запроса
        /// </summary>
        /// <param name="requestQueue">Очередь Rabbit для отправки</param>
        /// <param name="type">Тип запроса</param>
        /// <param name="requestBody">Тело запроса</param>
        /// <returns>Результат выполнения запроса</returns>
        public void SendRequest(string requestQueue, string type, byte[] requestBody)
        {
            // Создание очереди запросов на сервер (если сервер еще не стартовал или очередь была удалена)
            channel.QueueDeclare(queue: requestQueue, durable: false, exclusive: false, autoDelete: false);
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.Type = type;
            props.AppId = applicationId;
            channel.BasicPublish(exchange: "", routingKey: requestQueue, basicProperties: props, body: requestBody);
        }

        /// <summary>
        /// Подписка на оповещения
        /// </summary>
        /// <param name="announceExchange">Обмен для прослушивания оповещений</param>
        /// <param name="onReceived">Метод обработки сообщений оповещений</param>
        public void SubscribeToAnnounce(string announceExchange, Func<string, byte[], Task> onReceived)
        {
            string announceQueue = channel.QueueDeclare().QueueName;
            channel.QueueBind(announceQueue, announceExchange, "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                // Рассматриваются только чужие оповещения
                await onReceived.Invoke(args.BasicProperties.Type, args.Body);
            };

            channel.BasicConsume(announceQueue, true, consumer);
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            tockenSource?.Cancel();
            channel?.Dispose();
            connection?.Dispose();
        }
    }
}