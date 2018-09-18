using ArticlesClient.Clients.Rabbit;
using ArticlesClient.Clients.Rabbit.Converters;
using ArticlesClient.Clients.Wcf;
using ArticlesClient.Utils;
using RabbitMQ.Client;
using System;
using System.Configuration;
using System.Windows;
using ArticlesClient.Commands;

namespace ArticlesClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Фабрика клиентов для получения данных.
        /// </summary>
        public DataClientsFactory ReaderFactory { get; private set; }

        /// <summary>
        /// Клиент RabbitMQ.
        /// </summary>
        internal RabbitClient RabbitClient { get; private set; }

        /// <summary>
        /// Инициализация приложения.
        /// </summary>
        /// <param name="e">Аргументы события старта приложения.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            AutomapHelper.InitMapping();
            base.OnStartup(e);
            InitConnections();

            // Финализация лиента должа проиходить как при обычном выходе из приложения, так и при аварийном
            Dispatcher.ShutdownStarted += (sender, args) => RabbitClient?.Dispose();
            Dispatcher.UnhandledException +=(sender, args) => RabbitClient?.Dispose();
        }

        /// <summary>
        /// Инициализация способов соединений с сервером.
        /// </summary>
        private void InitConnections()
        {
            ReaderFactory = new DataClientsFactory(() => new WcfDataClient());

            var rabbitConnectionFactory = new ConnectionFactory
            {
                Uri = new Uri(ConfigurationManager.ConnectionStrings["RabbitConnection"].ConnectionString),
                AutomaticRecoveryEnabled = true
            };

            // Соединение с Rabbit формируется и закрывается на уровне приложения. Поэтому провайдер не владеет им
            string applicationd = Guid.NewGuid().ToString();
            var provider = new RabbitRequestProvider(rabbitConnectionFactory.CreateConnection(), applicationd);

            string requestsQueue = ConfigurationManager.AppSettings["RabbitRequestsQueue"];
            string announceExchange = ConfigurationManager.AppSettings["RabbitAnnounceExchange"];
            RabbitClient = new RabbitClient(provider, new AnnounceCommandsInvoker(), new JsonMessageBodyConverter(), requestsQueue, announceExchange);
        }
    }
}
