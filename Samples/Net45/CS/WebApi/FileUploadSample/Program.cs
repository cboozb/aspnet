using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace FileUploadSample
{
    /// <summary>
    /// This sample illustrates how to upload files to an ApiController using HttpClient
    /// </summary>
    class Program
    {
        const int BufferSize = 1024;

        static readonly Uri _baseAddress = new Uri("http://localhost:50231/");
        static readonly string _filename = "Sample.xml";

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

                // Set parameters for uploading large files
                config.MaxReceivedMessageSize = 16L * 1024 * 1024 * 1024;
                config.ReceiveTimeout = TimeSpan.FromMinutes(20);
                config.TransferMode = TransferMode.StreamedRequest;

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
        /// Runs an HttpClient uploading files using MIME multipart to the controller.
        /// The client uses a progress notification message handler to report progress. 
        /// </summary>
        static async void RunClient()
        {
            // Create a progress notification handler
            ProgressMessageHandler progress = new ProgressMessageHandler();
            progress.HttpSendProgress += ProgressEventHandler;

            // Create an HttpClient and wire up the progress handler
            HttpClient client = HttpClientFactory.Create(progress);

            // Set the request timeout as large uploads can take longer than the default 2 minute timeout
            client.Timeout = TimeSpan.FromMinutes(20);

            // Open the file we want to upload and submit it
            using (FileStream fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, useAsync: true))
            {
                // Create a stream content for the file
                StreamContent content = new StreamContent(fileStream, BufferSize);

                // Create Multipart form data content, add our submitter data and our stream content
                MultipartFormDataContent formData = new MultipartFormDataContent();
                formData.Add(new StringContent("Me"), "submitter");
                formData.Add(content, "filename", _filename);

                // Post the MIME multipart form data upload with the file
                Uri address = new Uri(_baseAddress, "/api/fileupload");
                HttpResponseMessage response = await client.PostAsync(address, formData);

                FileResult result = await response.Content.ReadAsAsync<FileResult>();
                Console.WriteLine("{0}Result:{0}  Filename:  {1}{0}  Submitter: {2}", Environment.NewLine, result.FileNames.FirstOrDefault(), result.Submitter);
            }
        }

        static void ProgressEventHandler(object sender, HttpProgressEventArgs eventArgs)
        {
            // The sender is the originating HTTP request   
            HttpRequestMessage request = sender as HttpRequestMessage;

            // Write different message depending on whether we have a total length or not   
            string message;
            if (eventArgs.TotalBytes != null)
            {
                message = String.Format("  Request {0} uploaded {1} of {2} bytes ({3}%)",
                    request.RequestUri, eventArgs.BytesTransferred, eventArgs.TotalBytes, eventArgs.ProgressPercentage);
            }
            else
            {
                message = String.Format("  Request {0} uploaded {1} bytes",
                    request.RequestUri, eventArgs.BytesTransferred, eventArgs.TotalBytes, eventArgs.ProgressPercentage);
            }

            // Write progress message to console   
            Console.WriteLine(message);
        }
    }
}
