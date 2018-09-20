using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ArticlesWeb.Clients;
using ArticlesWeb.Clients.Rabbit;
using ArticlesWeb.Clients.Rabbit.Converters;
using ArticlesWeb.Clients.Wcf;
using ArticlesWeb.Hubs;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;

namespace ArticlesWeb
{
    /// <summary>
    /// Стартовая настройка приложения
    /// </summary>
    public class Startup
    {
        private string applicationId;

        /// <summary>
        /// Идентификатор приложения
        /// </summary>
        private string ApplicationId => applicationId ?? (applicationId = new Guid().ToString());

        /// <summary>
        /// Настройки запуска приложения.
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Контейнер разрешения зависимостей.
        /// </summary>
        private IContainer ApplicationContainer { get; set; }

        private IConnection rabbitConnection;
        private IModel rabbitChannel;

        /// <summary>
        /// Канал подключения к RabbitMQ
        /// </summary>
        private IModel RabbitChannel
        {
            get
            {
                if (rabbitChannel == null)
                {
                    var rabbitConnectionFactory = new ConnectionFactory
                    {
                        Uri = new Uri(Configuration.GetSection("Rabbit").GetValue<string>("Uri")),
                        AutomaticRecoveryEnabled = true
                    };

                    rabbitConnection = rabbitConnectionFactory.CreateConnection();
                    rabbitChannel = rabbitConnection.CreateModel();
                }
                return rabbitChannel;
            }
        }

        /// <summary>
        /// Обработчик запуска приложения.
        /// </summary>
        /// <param name="configuration">Конфигурация запуска приложения.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Метод настройки сервисов и контейнеров
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <returns>Провайдер сервисов</returns>
        // ReSharper disable once UnusedMember.Global
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //string connection = Configuration.GetConnectionString("ClientsConnection");
            //services.AddDbContext<ClientsContext>(options => options.UseSqlServer(connection));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ArticlesApp/dist"; });
            services.AddSignalR()
                .AddJsonProtocol(options => { options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver(); });
            //services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            //{
            //    builder
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .WithOrigins("http://localhost:63366");
            //}));

            return BuildAutofacServiceProvider(services);
        }

        /// <summary>
        /// Построение профайдера сервисов на базе Autofac
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <returns>Провайдер сервисов</returns>
        private AutofacServiceProvider BuildAutofacServiceProvider(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<WcfDataContext>().As<IReadContext>();
            builder.Register(GetRabbitContext).As<IUpdateContext>();
            builder.RegisterType<JsonMessageBodyConverter>().As<IMessageBodyConverter>();

            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        /// <summary>
        /// Метод формирования контекста RabbitMQ.
        /// </summary>
        /// <param name="container">Текущий контейнер разрешения зависимостей</param>
        /// <returns>Сформированный контекст RabbitMQ</returns>
        private IUpdateContext GetRabbitContext(IComponentContext container)
        {
            IConfigurationSection rabbitSection = Configuration.GetSection("Rabbit");
            string requestQueue = rabbitSection.GetValue<string>("RequestQueue");
            string announceExchange = rabbitSection.GetValue<string>("AnnounceExchange");
            RabbitRequestProvider provider = new RabbitRequestProvider(RabbitChannel, ApplicationId);

            return new RabbitContext(provider, container.Resolve<IMessageBodyConverter>(), requestQueue, announceExchange);
        }

        /// <summary>
        /// Первоначальная конфигурация приложения
        /// </summary>
        /// <param name="app">Конструктор приложения</param>
        /// <param name="env">Переменная окружения</param>
        /// <param name="applicationLifetime">Переменная жизненного цикла приложения</param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(Path.Combine(env.ContentRootPath, @"Logs\log-.txt"),
                    LogEventLevel.Verbose,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:HH:mm:ss:ffff} [{Level}]: {Message}{NewLine}{Exception}")
                .CreateLogger();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseMvc();
            //app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<UpdatesHub>("/updates");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ArticlesApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            applicationLifetime.ApplicationStarted.Register(OnStartup, applicationLifetime.ApplicationStopping);
            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        /// <summary>
        /// Завершение запуска сервера
        /// </summary>
        /// <param name="cancellationTocken">Маркер завершения работы сервера</param>
        private void OnStartup(object cancellationTocken)
        {
            Task.Run(async () =>
            {
                // Подписка на оповещения об обновлениях
                var hubContext = ApplicationContainer.Resolve<IHubContext<UpdatesHub>>();
                IUpdateContext rabbitContext = ApplicationContainer.Resolve<IUpdateContext>();
                await rabbitContext.SubscribeToUpdatesAsync(hubContext, (CancellationToken)cancellationTocken);
            });
        }

        /// <summary>
        /// Освобождение ресурсов при завершении работы сервера
        /// </summary>
        private void OnShutdown()
        {
            rabbitChannel?.Close();
            rabbitChannel?.Dispose();

            rabbitConnection?.Close();
            rabbitConnection?.Dispose();
        }
    }
}
