using System;

namespace ArticlesService.Services.Models
{
    /// <summary>
    /// Конструктор результата операции.
    /// </summary>
    public static class ResultBuilder
    {
        /// <summary>
        /// Формирование результат успешной операции.
        /// </summary>
        /// <typeparam name="T">Тип передаваемых данных.</typeparam>
        /// <param name="data">передаваемые в результате данные.</param>
        /// <returns>Результат операции с флагом успешного выполнения.</returns>
        public static ResultDto<T> Success<T>(T data)
        {
            return new ResultDto<T>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Формирование результат об ошибке выполнения операции.
        /// </summary>
        /// <typeparam name="T">Тип передаваемых данных.</typeparam>
        /// <param name="exception">Текст сообщения об ошибке.</param>
        /// <returns>Результат операции с флагом провала выполнения.</returns>
        public static ResultDto<T> Fault<T>(string exception)
        {
            return new ResultDto<T>
            {
                Success = false,
                Message = exception
            };
        }

    }
}