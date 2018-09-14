using System;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Исполнитель команд сообщений.
    /// </summary>
    public class CommandExecutor : ICommandExecutor
    {
        private readonly Func<byte[], byte[]> method;

        /// <summary>
        /// Имя исполнямой команды.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Выполнение команды.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Execute(byte[] input)
        {
            return method.Invoke(input);
        }

        public CommandExecutor(string name, Func<byte[], byte[]> method)
        {
            Name = name;
            this.method = method;
        }
    }
}