namespace Articles.Services.Executors
{
    /// <summary>
    /// Интерфейс провайдера исполнителей
    /// </summary>
    public interface IExecutorsProvider
    {
        /// <summary>
        /// Получить исполнителя по его имени
        /// </summary>
        /// <param name="name">Имя исполнителя</param>
        /// <returns>Возвращает запрашиваемого исполнителя</returns>
        ICommandExecutor Get(string name);
    }
}
