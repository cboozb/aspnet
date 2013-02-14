using Owin;
using System.Web.Http;

namespace WebApi
{
    // Note the Web.Config owin:HandleAllRequests setting that is used to direct all requests to your OWIN application.
    // Alternatively you can specify routes in the global.asax file.
    public class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder builder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute("Default", "{controller}/{customerID}", new { controller = "Customer", customerID = RouteParameter.Optional });

            config.Formatters.XmlFormatter.UseXmlSerializer = true;
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            // config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

            builder.UseHttpServer(config);
        }
    }
}