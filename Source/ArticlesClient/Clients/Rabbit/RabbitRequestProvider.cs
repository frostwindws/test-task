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
        private readonly IModel channel;
        private readonly BlockingCollection<byte[]> responseQueue = new BlockingCollection<byte[]>();
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();
        private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Конструктор запроса.
        /// </summary>
        /// <param name="connection">Используемое соединение.</param>
        public RabbitRequestProvider(IConnection connection)
        {
            this.connection = connection;
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
        /// <param name="announceQueue">Очередь для прослушивания оповещений</param>
        /// <param name="onReceived">Метод обработки сообщений оповещений</param>
        public void SubscribeToAnnounce(string announceExchange, string announceQueue, Action<string, byte[]> onReceived)
        {
            channel.QueueBind(announceQueue, announceExchange, "");
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) => { onReceived.Invoke(args.BasicProperties.Type, args.Body); };
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
