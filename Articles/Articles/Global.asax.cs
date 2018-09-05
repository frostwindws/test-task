using System;
using System.Web;
using Articles.Models;
using Articles.Services;
using Autofac;
using Autofac.Integration.Wcf;

namespace Articles
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ArticlesService>();
            builder.RegisterType<ArticlesRepository>().As<IArticlesRepository>();

            builder.RegisterType<CommentsService>();
            builder.RegisterType<CommentsRepository>().As<ICommentsRepository>();

            AutofacHostFactory.Container = builder.Build();
        }
    }
}