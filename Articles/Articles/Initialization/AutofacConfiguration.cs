using System.ServiceModel;
using Articles.Models;
using Articles.Services;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.Wcf;

namespace Articles.Initialization
{
    /// <summary>
    /// Класс конфигурации библиотеки Autofac
    /// </summary>
    internal static class AutofacConfiguration
    {
        public static void Init()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LoggerInterceptor>();
            builder.RegisterType<ArticlesService>()
                .As<IArticlesService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));
            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>();

            builder.RegisterType<CommentsService>()
                .As<ICommentsService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor)); ;
            builder.RegisterType<CommentsRepository>().As<ICommentsRepository>();

            AutofacHostFactory.Container = builder.Build();
        }
    }
}