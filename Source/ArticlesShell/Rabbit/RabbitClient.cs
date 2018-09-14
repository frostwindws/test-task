using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ArticlesShell.Conververters;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArticlesShell.Rabbit
{
    /// <summary>
    /// Клиент обращения к RabbitMQ
    /// </summary>
    public class RabbitClient : IDisposable
    {
        private readonly IConnection connection;
        private readonly string queueName;
        private readonly IMessageBodyConverter bodyConverter;
        private readonly BlockingCollection<byte[]> responseQueue = new BlockingCollection<byte[]>();
        private readonly CancellationTokenSource tockenSource = new CancellationTokenSource();
        private readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Конструктор запроса.
        /// </summary>
        /// <param name="connection">Используемое соединение.</param>
        /// <param name="queueName">Имя используемой очереди.</param>
        /// <param name="bodyConverter">Конвертер тела сообщения</param>
        public RabbitClient(IConnection connection, string queueName, IMessageBodyConverter bodyConverter)
        {
            this.connection = connection;
            this.queueName = queueName;
            this.bodyConverter = bodyConverter;
        }

        /// <summary>
        /// Отправка запроса.
        /// </summary>
        /// <typeparam name="T">Тип ожидаемого результата.</typeparam>
        /// <param name="type">Тип запроса.</param>
        /// <param name="requestObject">Объект, передаваемый в запросе.</param>
        /// <returns>Результат ответа на запрос.</returns>
        public T SendRequest<T>(string type, object requestObject)
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

                // Максимальное время ожидания ответа - 30 секунд
                byte[] response = responseQueue.Take(tockenSource.Token);
                return bodyConverter.FromBody<T>(response);
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            connection?.Dispose();
            responseQueue?.Dispose();
            tockenSource?.Dispose();
        }
    }
}
