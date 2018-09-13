using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using ArticlesClient.Clients.Rabbit.Converters;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArticlesClient.Clients.Rabbit
{
    /// <summary>
    /// Провайдер запросов к RabbitMQ
    /// </summary>
    public class RabbitRequestProvider : IDisposable
    {
        private readonly IConnection connection;
        private readonly bool ownConnection;
        private readonly string queueName;
        private readonly IMessageBodyConverter bodyConverter;
        private readonly BlockingCollection<byte[]> responseQueue = new BlockingCollection<byte[]>();
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();
        private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Конструктор запроса
        /// </summary>
        /// <param name="connection">Используемое соединение</param>
        /// <param name="ownConnection">Флаг владения провайдером соединением (если True, то провайдер отвечает за его освобождение)</param>
        /// <param name="queueName">Имя используемой очереди</param>
        /// <param name="bodyConverter">Конвертер, используемый для преобразований</param>
        public RabbitRequestProvider(IConnection connection, bool ownConnection, string queueName, IMessageBodyConverter bodyConverter)
        {
            this.connection = connection;
            this.ownConnection = ownConnection;
            this.queueName = queueName;
            this.bodyConverter = bodyConverter;
        }

        /// <summary>
        /// Отправка запроса
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого результата</typeparam>
        /// <param name="type">Тип запроса</param>
        /// <param name="requestObject">Объект, передаваемый в запросе</param>
        /// <returns>Результат ответа на запрос</returns>
        public async Task<T> SendRequest<T>(string type, object requestObject)
        {
            using (var channel = connection.CreateModel())
            {
                // Создание очереди запросов на сервер (если сервер еще не стартовал или очередь была удалена)
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

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
                byte[] body = bodyConverter.ToBody(requestObject);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: props, body: body);

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

                return bodyConverter.FromBody<T>(response);
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            tockenSource?.Cancel();
            if (ownConnection)
            {
                // Соединение освобождается только если провайдер им владеет
                connection?.Dispose();
            }
        }
    }
}
