using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ODataService.WebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enables OData support by adding an OData route and enabling querying support for OData.
            // Action selector and odata media type formatters will be registered in per-controller configuration only
            config.Routes.MapODataRoute(
                routeName: "OData", 
                routePrefix: null, 
                model: ModelBuilder.GetEdmModel());
            config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
