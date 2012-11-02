using System;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using AttributeRouting.Web.Http.SelfHost;
using AttributeRoutingSample.Models;

namespace AttributeRoutingSample
{
    /// <summary>
    /// This sample illustrates using AttributeRouting with ASP.NET Web API.
    /// </summary>
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:50231/");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
                config.HostNameComparisonMode = HostNameComparisonMode.Exact;

                // Set up Attribute Routing
                config.Routes.MapHttpAttributeRoutes(attributeRoutingConfig =>
                {
                    // Get list of assemblies we search
                    IAssembliesResolver assemblyResolver = config.Services.GetAssembliesResolver();

                    // Add routes for all controllers in these assemblies
                    foreach (Assembly assembly in assemblyResolver.GetAssemblies())
                    {
                        attributeRoutingConfig.AddRoutesFromAssembly(assembly);
                    }
                });

                // Create server
                server = new HttpSelfHostServer(config);

                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress);

                // Run HttpClient issuing requests
                RunClient();

                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();
            }
            finally
            {
                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
        }

        /// <summary>
        /// Runs an HttpClient issuing sample requests against controller using AttributeRouting.
        /// </summary>
        static void RunClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = _baseAddress;

            // Get all contacts
            HttpResponseMessage response = client.GetAsync("/contacts").Result;
            response.EnsureSuccessStatusCode();
            string content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("\nAll contacts returned: {0}", content);

            // Get contact with default id = 1
            response = client.GetAsync("/contactOrDefault").Result;
            response.EnsureSuccessStatusCode();
            content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("\nContact with default value of id = 1: {0}", content);

            // Get contact with out of range id = 4
            response = client.GetAsync("/contactRange/4").Result;
            content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("\nContact with out of range id = 4: {0}", content);

            // Post new contact
            Contact contact = new Contact
            {
                Email = "new@example.com",
                Name = "new",
                Phone = "xxx xxx xxxx"
            };
            response = client.PostAsJsonAsync("/contact", contact).Result;
            response.EnsureSuccessStatusCode();
            content = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine("\nNew contact: {0}", content);

            // Update contact
            response = client.PutAsJsonAsync("/contact/1", contact).Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nUpdated contact with id = 1 succeeded");
        }
    }
}
