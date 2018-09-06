using System;
using Castle.DynamicProxy;
using Serilog;

namespace Articles.Initialization
{
    public class LoggerInterceptor :IInterceptor
    {
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