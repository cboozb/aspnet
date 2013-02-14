using Microsoft.Owin.Host.SystemWeb;
using Owin;
using System;
using System.Web.Routing;

namespace AspNetRoutes
{
    public class Global : System.Web.HttpApplication
    {
        // How to hook OWIN pipelines into the normal Asp.Net route table side by side with other components.
        protected void Application_Start(object sender, EventArgs e)
        {
            // TODO: In recent builds MapOwinRoute now uses AspNet route pattern matching.
            // MapOwinPath should be used for the basic path match below.

            // Paths that start with /owin will be directed to our startup class.
            RouteTable.Routes.MapOwinRoute("/owin");

            RouteTable.Routes.MapOwinRoute("/special", builder =>
            {
                builder.UseHandler(new StartupExtensions.OwinHandlerAsync(OwinApp2.Invoke));
            });
        }
    }
}