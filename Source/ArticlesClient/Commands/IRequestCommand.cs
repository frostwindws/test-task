using System;
using ArticlesClient.Models;

namespace ArticlesClient.Commands
{
    /// <summary>
    /// Интерфейс команды обработчика информации об обновлении данных.
    /// </summary>
    /// <typeparam name="T">Тип обновленных данных.</typeparam>
    internal interface IRequestCommand<T>
    {
        /// <summary>
        /// Выполняемый метод.
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Обновленные даные.</param>
        void Execute(ViewDataContainer viewData, T record);
    }
}