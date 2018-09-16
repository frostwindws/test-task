using Articles.Models;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Интерфейс команды на обновление данных.
    /// </summary>
    /// <typeparam name="T">Тип обновляемой модели данных.</typeparam>
    public interface IRequestCommand<T>
    {
        /// <summary>
        /// Выполняемый метод.
        /// </summary>
        /// <param name="context">Контекст для работы.</param>
        /// <param name="record">Объект модели запроса на обновление данных.</param>
        /// <param name="validator">Валидатор для проверки данных.</param>
        /// <returns>Данные, сформированные в процессе выполнения команды</returns>
        T Execute(IDataContext context, T record, IModelValidator<T> validator);
    }
}