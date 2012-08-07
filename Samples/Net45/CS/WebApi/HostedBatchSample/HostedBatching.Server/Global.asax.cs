using HostedBatching.Handler;
using System;
using System.Web.Http;

namespace HostedBatching
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var config = GlobalConfiguration.Configuration;

            // add normal message handlers
            config.MessageHandlers.Add(new SampleMessageHandler("Alpha"));
            config.MessageHandlers.Add(new SampleMessageHandler("Beta"));

            // create a new batch message handler and map batch routes to it
            // the batch handler accept a HttpConfiguration and create a BatchServer internally
            var batchHandler = new BatchHandler(config);

            config.Routes.MapHttpRoute(
                "batch",
                "api/batch",
                null,
                null,
                batchHandler);

            config.Routes.MapHttpRoute(
                "default",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
        }
    }
}