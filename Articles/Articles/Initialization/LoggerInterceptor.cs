using System;
using Castle.DynamicProxy;
using Serilog;

namespace Articles.Initialization
{
    /// <summary>
    /// Перехватчик запросов к методам сервисов для реализации автоматического логирования действий и ошибок
    /// </summary>
    public class LoggerInterceptor :IInterceptor
    {
        /// <summary>
        /// Обработчик перехвата вызова методы
        /// </summary>
        /// <param name="invocation">Данные вызываемого метода</param>
        public void Intercept(IInvocation invocation)
        {
            Log.Verbose($"{invocation.TargetType.Name}: executing {invocation.Method.Name}");
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error accured while executing {invocation.TargetType.Name}.{invocation.Method.Name}");
                throw;
            }
        }
    }
}