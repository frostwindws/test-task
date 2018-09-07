using System;
using System.Web;
using Articles.Initialization;

namespace Articles
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            MapperConfiguration.Init();
            AutofacConfiguration.Init();
            SerilogConfiguration.Init();
        }
    }
}