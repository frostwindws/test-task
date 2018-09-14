using System;
using System.Configuration;
using System.Data;
using Articles.Dapper;
using Articles.Messaging;
using Articles.Messaging.Converters;
using Articles.Messaging.Rabbit;
using Articles.Models;
using Articles.Nhibernate;
using Articles.Nhibernate.Mapping;
using Articles.Services;
using Articles.Services.Executors;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.Wcf;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using Npgsql;
using RabbitMQ.Client;
using Serilog;

namespace Articles.Initialization
{
    /// <summary>
    /// Класс конфигурации библиотеки Autofac.
    /// </summary>
    internal static class DependencyConfiguration
    {
        private static readonly string contextMode = ConfigurationManager.AppSettings["ContextMode"];

        private static Func<IDataContext> ContextBulder
        {
            get
            {
                switch (contextMode)
                {
                    case "NHibernate":
                        return BuildNhibernateContext;
                    case "Dapper":
                        return BuildDapperContext;
                    default:
                        throw new InvalidExpressionException("Invalid ContextMode setting avalaible settings: NHibernate or Dapper");
                }
            }
        }

        /// <summary>
        /// Инициализация настроек Autofac.
        /// </summary>
        public static void InitWcfServicesFactory()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<LoggerInterceptor>();

            // Классы, используемые при создании сервиса
            builder.Register(c => ContextBulder()).As<IDataContext>();
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

        private static NhibernateContext BuildNhibernateContext()
        {
            var configuration = new NHibernate.Cfg.Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionStringName = "PostgreConnection";
                    db.Dialect<PostgreSQLDialect>();
                });

            var mapper = new ModelMapper();
            mapper.AddMappings(new[] { typeof(ArticleMap), typeof(CommentMap) });
            configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            using (var factory = configuration.BuildSessionFactory())
            {
                return new NhibernateContext(factory.OpenSession());
            }
        }

        /// <summary>
        /// Получение фабрики для сервисов прослушивания сообщений.
        /// </summary>
        /// <returns></returns>
        public static ListenerServicesFactory GetListenerServicesFactory()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<RabbitListenerService>().As<IListenerService>();
            builder.Register(c => new ListenersFactory(ListenersBuilder)).As<ListenersFactory>();
            builder.Register(c => new DataContextFactory(ContextBulder)).As<DataContextFactory>();
            builder.RegisterType<ServiceExecutorsProvider>().As<IExecutorsProvider>();
            builder.RegisterType<ServiceExecutorsProvider>().As<IExecutorsProvider>();
            builder.RegisterType<JsonMessageBodyConverter>().As<IMessageBodyConverter>();
            builder.RegisterType<ArticlesValidator>().As<IModelValidator<Article>>();
            builder.RegisterType<CommentsValidator>().As<IModelValidator<Comment>>();

            return new ListenerServicesFactory(builder.Build());
        }

        /// <summary>
        /// Функция построение контекста данных.
        /// </summary>
        /// <returns></returns>
        private static IDataContext BuildDapperContext()
        {
            return new DapperContext(CreateNpgsqlConnection());
        }

        /// <summary>
        /// Функция построения слушателя запросов.
        /// </summary>
        private static IRequestListener ListenersBuilder()
        {
            try
            {
                return new RabbitListener(new ConnectionFactory { Uri = GetListenerUri() }, GetRabbitQueueName());
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while connecting to Rabbit MQ Server. Listener wasn't created");
                throw;
            }
        }

        /// <summary>
        /// Получение Uri для обращения к RabbitMQ.
        /// </summary>
        /// <returns>Сформированный Uri.</returns>
        private static Uri GetListenerUri()
        {
            return new Uri(ConfigurationManager.ConnectionStrings["RabbitConnection"].ConnectionString);
        }

        /// <summary>
        /// Получение настроек RabbitMQ.
        /// </summary>
        /// <returns></returns>
        private static string GetRabbitQueueName()
        {
            return ConfigurationManager.AppSettings["RabbitQueue"];
        }

        /// <summary>
        /// Создание соединения для работы с PostgreSQL.
        /// </summary>
        /// <returns>Новое соединение.</returns>
        private static IDbConnection CreateNpgsqlConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreConnection"].ConnectionString;
            return new NpgsqlConnection(connectionString);
        }

    }
}