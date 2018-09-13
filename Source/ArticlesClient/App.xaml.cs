using System;
using System.Configuration;
using System.Windows;
using ArticlesClient.Clients.Rabbit;
using ArticlesClient.Clients.Rabbit.Converters;
using ArticlesClient.Clients.Wcf;
using ArticlesClient.Utils;
using RabbitMQ.Client;

namespace ArticlesClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Фабрика формирования соединений с RabbitMQ
        /// </summary>
        private ConnectionFactory rabbitConnectionFactory;

        /// <summary>
        /// Текущее соединение с RabbitMQ
        /// </summary>
        private IConnection rabbitConnection;

        /// <summary>
        /// Используемая для запросов через RabbitMQ очередь
        /// </summary>
        private string RoutingKey => ConfigurationManager.AppSettings["RabbitRoutingKey"];

        /// <summary>
        /// Фабрика клиентов для получения данных
        /// </summary>
        public DataClientsFactory ReaderFactory { get; private set; }

        /// <summary>
        /// Фабрика клиентов для чтения данных
        /// </summary>
        public DataClientsFactory WriterFactory { get; private set; }

        /// <summary>
        /// Инициализация приложения
        /// </summary>
        /// <param name="e">Аргументы события старта приложения</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            AutomapHelper.InitMapping();
            base.OnStartup(e);
            InitFactories();
        }

        private void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            rabbitConnection?.Close();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            // Соединение обязано закрываться как при нормальном выходе из приложения, так и при аварийном
            rabbitConnection?.Close();
        }

        /// <summary>
        /// Инициализация используемых фабрик
        /// </summary>
        private void InitFactories()
        {
            ReaderFactory = new DataClientsFactory(() => new WcfDataClient());
            WriterFactory = new DataClientsFactory(() => InitRabbitClient());
            rabbitConnectionFactory = new ConnectionFactory
            {
                Uri = new Uri(ConfigurationManager.ConnectionStrings["RabbitConnection"].ConnectionString)
            };
        }

        /// <summary>
        /// Инициализация клинета Rabbit MQ
        /// </summary>
        /// <returns>Сформированный клиент Rabbit MQ</returns>
        private RabbitClient InitRabbitClient()
        {
            // Если по какой либо причине соединение было разорвано - оно открывается снова
            if (rabbitConnection == null || !rabbitConnection.IsOpen)
            {
                rabbitConnection = rabbitConnectionFactory.CreateConnection();
            }

            // Соединение с Rabbit формируется и закрывается на уровне приложения. Поэтому провайдер не владеет им
            var provider = new RabbitRequestProvider(rabbitConnection, false, RoutingKey, new JsonMessageBodyConverter());
            return new RabbitClient(provider);
        }
    }
}
