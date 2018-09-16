using ArticlesService.Initialization;
using ArticlesService.Messaging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;

namespace ArticlesService
{
    class ServiceApplication
    {
        static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            args = args.Where(arg => arg != "--console").ToArray();
            IWebHost host = CreateWebHostBuilder(isService, args).Build();

            if (isService)
            {
                ServiceBase.Run(new ArticlesWebHostService(host));
            }
            else
            {
                host.Run();
            }


            Log.Information("Articles server has started");
        }

        public static IWebHostBuilder CreateWebHostBuilder(bool isService, string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                 //.ConfigureAppConfiguration((context, config) =>
                 //{
                 //    // Configure the app here.
                 //})
                 .UseStartup<Startup>();

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                builder.UseContentRoot(pathToContentRoot);
            }

            return builder;
        }
    }
}
