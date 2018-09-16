using System;
using System.Web;
using System.Web.Hosting;
using Articles.Initialization;
using Articles.Messaging;
using Serilog;

namespace Articles
{
    /// <summary>
    /// Глобальный класс веб приложения.
    /// </summary>
    public class Global : HttpApplication
    {
        /// <summary>
        /// Запуск приложения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            MapperConfiguration.Init();
            DependencyConfiguration.InitWcfServicesFactory();
            SerilogConfiguration.Init();
            StartBackgroundListener();

            Log.Information("Articles server has started");
        }

        /// <summary>
        /// Запуск прослушивания очереди запросов.
        /// </summary>
        private void StartBackgroundListener()
        {
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                using (var listenerServicesFactory = DependencyConfiguration.GetListenerServicesFactory())
                {
                    IListenerService service = listenerServicesFactory.GetListenerService();
                    await service.StartListenning(ct);
                }
            });
        } 

        /// <summary>
        /// Остановка приложения.
        /// </summary>
        /// <param name="sender">Источник события.</param>
        /// <param name="e">Аргументы события.</param>
        protected void Application_End(object sender, EventArgs e)
        {
            Log.Information("Articles server has stopped");
        }
    }
}