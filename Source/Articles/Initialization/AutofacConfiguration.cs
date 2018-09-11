using System.Configuration;
using System.Data;
using Articles.Dapper;
using Articles.Models;
using Articles.Services;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.Wcf;
using Npgsql;

namespace Articles.Initialization
{
    /// <summary>
    /// Класс конфигурации библиотеки Autofac
    /// </summary>
    internal static class AutofacConfiguration
    {
        /// <summary>
        /// Инициализация настроек Autofac
        /// </summary>
        public static void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LoggerInterceptor>();

            // Классы, используемые при создании сервиса
            builder.Register(c => new DapperContext(CreateNpgsqlConnection())).As<IDataContext>();
            builder.RegisterType<ArticlesValidator>().As<IModelValidator<Article>>();
            builder.RegisterType<CommentsValidator>().As<IModelValidator<Comment>>();

            // Перехват методов сервиса
            builder.RegisterType<ArticlesService>()
                .As<IArticlesService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));

            builder.RegisterType<CommentsService>()
                .As<ICommentsService>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggerInterceptor));
            
            AutofacHostFactory.Container = builder.Build();
        }

        /// <summary>
        /// Создание соединения для работы с PostgreSQL
        /// </summary>
        /// <returns>Новое соединение</returns>
        private static IDbConnection CreateNpgsqlConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            return new NpgsqlConnection(connectionString);
        }
    }
}