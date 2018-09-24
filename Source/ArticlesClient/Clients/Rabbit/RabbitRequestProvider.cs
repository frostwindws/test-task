using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Провайдер запросов к RabbitMQ.
    /// </summary>
    internal class RabbitRequestProvider : IDisposable
    {
        private readonly IConnection connection;
        private readonly string applicationId;
        private readonly IModel channel;
        private readonly BlockingCollection<byte[]> responseQueue = new BlockingCollection<byte[]>();
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();
        private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(10);

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
        public async Task<byte[]> SendRequest(string requestQueue, string type, byte[] requestBody)
        {
            // Создание очереди запросов на сервер (если сервер еще не стартовал или очередь была удалена)
            channel.QueueDeclare(queue: requestQueue, durable: false, exclusive: false, autoDelete: false);

            // Подписка на событие получения ответа
            var replyTo = channel.QueueDeclare().QueueName;
            var correlationId = Guid.NewGuid().ToString();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                if (args.BasicProperties.CorrelationId == correlationId)
                {
                    responseQueue.Add(args.Body);
                }
            };

            // Формирование запроса
            var props = channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.Type = type;
            props.ReplyTo = replyTo;
            props.AppId = applicationId;
            channel.BasicPublish(exchange: "", routingKey: requestQueue, basicProperties: props, body: requestBody);

            // Получение ответа
            channel.BasicConsume(consumer, replyTo);
            byte[] response = null;

            // Максимальное время ожидания ответа - 30 секунд
            Task requestTask = Task.Run(() =>
            {
                response = responseQueue.Take(tockenSource.Token);
            });
            Task timeoutTask = Task.Delay(requestTimeout);
            await Task.WhenAny(requestTask, timeoutTask);
            if (timeoutTask.IsCompleted)
            {
                throw new CommunicationException($"Service hasn't responded in {requestTimeout.Seconds} seconds on your \"{type}\" request. It was accepted and will be executed latter. Please be patient");
            }

            return response;
        }

        /// <summary>
        /// Подписка на оповещения
        /// </summary>
        /// <param name="announceExchange">Обмен для прослушивания оповещений</param>
        /// <param name="onReceived">Метод обработки сообщений оповещений</param>
        public void SubscribeToAnnounce(string announceExchange, Action<string, byte[]> onReceived)
        {
            string announceQueue = channel.QueueDeclare().QueueName;
            channel.ExchangeDeclare(announceExchange, "fanout");
            channel.QueueBind(announceQueue, announceExchange, "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                if (args.BasicProperties.AppId != applicationId)
                {
                    // Рассматриваются только чужие оповещения
                    onReceived.Invoke(args.BasicProperties.Type, args.Body);
                }
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
