using System;
using System.Globalization;
using System.Windows.Data;

namespace ArticlesClient.Converters
{
    /// <summary>
    /// Конвертер флага в заголовок окна редактирования
    /// </summary>
    internal class EditorTitleConverter: IValueConverter
    {
        /// <summary>
        /// Конверт значения
        /// </summary>
        /// <param name="value">Значение флага</param>
        /// <param name="targetType">Целевой тип конвертации</param>
        /// <param name="parameter">Дополнительный параметр конвертации</param>
        /// <param name="culture">Информация о текущей культуре</param>
        /// <returns>Результат конвертации (Текст о редактировании, либо создании)</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? $"Add new {parameter}" : $"Edit {parameter}";
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
