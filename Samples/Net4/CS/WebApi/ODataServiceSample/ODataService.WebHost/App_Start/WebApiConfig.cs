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
            config.Formatters.Clear();

            // Enables OData support by adding an OData route and enabling OData controller and action selection, querying, and formatter support for OData. 
            config.EnableOData(ModelBuilder.GetEdmModel());

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
