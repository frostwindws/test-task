using ArticlesService.Initialization;
using ArticlesService.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Threading;
using System.Threading.Tasks;

namespace ArticlesService
{
    internal class ArticlesWebHostService : WebHostService
    {
        public IWebHost serviceHost = null;
        private static CancellationTokenSource tockenSource;

        public ArticlesWebHostService(IWebHost host) : base(host)
        {
            ServiceName = "ArticlesService";
            MapperConfiguration.Init();
            DependencyConfiguration.InitWcfServicesFactory();
            SerilogConfiguration.Init();
        }

        protected override void OnStarting(string[] args)
        {
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
            StartBackgroundListener();
        }

        protected override void OnStopping()
        {
            base.OnStopping();
        }

        /// <summary>
        /// Запуск прослушивания очереди запросов.
        /// </summary>
        static private void StartBackgroundListener()
        {
            Task.Run(async () =>
            {
                IListenerService service;
                using (var listenerServicesFactory = DependencyConfiguration.GetListenerServicesFactory())
                {
                    service = listenerServicesFactory.GetListenerService();
                }

                await service.StartListenning(tockenSource.Token);
            });
        }
    }
}
