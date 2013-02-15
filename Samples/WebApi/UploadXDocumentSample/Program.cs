using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Xml.Linq;

namespace UploadXDocumentSample
{
    /// <summary>
    /// Sample uploading an XDocument using PushStreamContent and HttpClient.
    /// </summary>
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:50231");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
                config.HostNameComparisonMode = HostNameComparisonMode.Exact;

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

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

        static async void RunClient()
        {
            // Create an HttpClient instance
            HttpClient client = new HttpClient();

            // Create a push content so that we can use XDocument.Save to a stream
            XDocument xDoc = XDocument.Load("Sample.xml", LoadOptions.None);
            PushStreamContent xDocContent = new PushStreamContent(
                (stream, content, context) =>
                {
                    // After save we close the stream to signal that we are done writing.
                    xDoc.Save(stream);
                    stream.Close();
                },
                "application/xml");

            // Send POST request to server and wait asynchronously for the response
            Uri address = new Uri(_baseAddress, "/api/book");
            HttpResponseMessage response = await client.PostAsync(address, xDocContent);

            // Ensure we get a successful response.
            response.EnsureSuccessStatusCode();

            // Read the response using XDocument as well
            Stream responseStream = await response.Content.ReadAsStreamAsync();
            XDocument xResponseDoc = XDocument.Load(responseStream);
            Console.WriteLine("Received response: {0}", xResponseDoc.ToString());
        }
    }
}