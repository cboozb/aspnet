using System;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Routing;
using System.Web.Http.SelfHost;
using System.Web.Http.Tracing;
using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Query;
using ODataService.Models;

namespace ODataService.SelfHost
{
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:50231/");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;

            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration configuration = new HttpSelfHostConfiguration(_baseAddress);
                configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                configuration.Formatters.Clear();

                // Enables OData support by adding an OData route and enabling OData controller and action selection, querying, and formatter support for OData. 
                configuration.EnableOData(ModelBuilder.GetEdmModel());

                //NOTE: Uncomment this if you want to enable diagnostic tracing
                //configuration.EnableSystemDiagnosticsTracing();

                // Create server
                server = new HttpSelfHostServer(configuration);

                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
            }
            finally
            {
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();

                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
        }
    }
}
