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
        private readonly string applicationId;
        private readonly IModel channel;
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();

        /// <summary>
        /// Конструктор запроса.
        /// </summary>
        /// <param name="channel">Канал подключения к Rabbit.</param>
        /// <param name="applicationId">Идентификатор сообщений (необходим для отбрасывания оповещений событий, источником которых было само приложение).</param>
        public RabbitRequestProvider(IModel channel, string applicationId)
        {
            this.applicationId = applicationId;
            this.channel = channel;
        }

        /// <summary>
        /// Отправка запроса.
        /// </summary>
        /// <param name="requestQueue">Очередь Rabbit для отправки.</param>
        /// <param name="type">Тип запроса.</param>
        /// <param name="requestBody">Тело запроса.</param>
        /// <returns>Результат выполнения запроса.</returns>
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
        /// Подписка на оповещения.
        /// </summary>
        /// <param name="announceExchange">Обмен для прослушивания оповещений.</param>
        /// <param name="onReceived">Метод обработки сообщений оповещений.</param>
        /// <param name="cancellationToken">Токен отмены подписки.</param>
        public async Task SubscribeToAnnounce(string announceExchange, Func<string, byte[], CancellationToken, Task> onReceived, CancellationToken cancellationToken)
        {
            string announceQueue = channel.QueueDeclare().QueueName;
            channel.ExchangeDeclare(announceExchange, "fanout");
            channel.QueueBind(announceQueue, announceExchange, "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                await onReceived.Invoke(args.BasicProperties.Type, args.Body, cancellationToken);
            };

            channel.BasicConsume(announceQueue, true, consumer);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            // Канал принадлежит приложению, поэтому не завкрывается
            tockenSource?.Cancel();
        }
    }
}