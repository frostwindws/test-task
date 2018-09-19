using System;
using System.IO;
using ArticlesWeb.Hubs;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;

namespace ArticlesWeb
{
    public class Startup
    {
        /// <summary>
        /// текущие настройки приложения
        /// </summary>
        private IConfiguration Configuration { get; }

        /// <summary>
        /// Контейнер разрешения зависимостей 
        /// </summary>
        private IContainer ApplicationContainer { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

        private AutofacServiceProvider BuildAutofacServiceProvider(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            // TODO: Настроить классы

            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
                .WriteTo.File(Path.Combine(env.WebRootPath, @"Logs\log-.txt"),
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
        }
    }
}
