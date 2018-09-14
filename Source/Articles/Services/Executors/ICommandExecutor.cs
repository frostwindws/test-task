using System;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Исполнитель команд сообщений.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Имя исполнямой команды.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Метод выполнения команды.
        /// </summary>
        /// <param name="input">Входные данные данные.</param>
        /// <returns></returns>
        byte[] Execute(byte[] input);
    }
}