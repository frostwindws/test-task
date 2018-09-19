using System;
using System.Text;
using Newtonsoft.Json;

namespace ArticlesWeb.Clients.Rabbit.Converters
{
    /// <summary>
    /// Конвертер тела сообщения с использованием JSON.
    /// </summary>
    public class JsonMessageBodyConverter: IMessageBodyConverter
    {
        /// <summary>
        /// Чтение тела сообщения.
        /// </summary>
        /// <typeparam name="T">Целевой тип.</typeparam>
        /// <param name="body">Тело сообщения.</param>
        /// <returns>Сформированный объект данных.</returns>
        public T FromBody<T>(byte[] body)
        {
            var jsonString = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Конвертация данных в тело сообщения.
        /// </summary>
        /// <param name="data">Исходные данные.</param>
        /// <returns>Тело сообщения.</returns>
        public byte[] ToBody(object data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(jsonString);
        }
    }
}