using System;
using System.IdentityModel.Selectors;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;
using ClientCertificateSample.Models;

namespace ClientCertificateSample
{
    /// <summary>
    /// This sample shows how to secure a web API server with mutual Certificates. It also shows how to authenticate and authorize based 
    /// on client's certificate.
    ///
    /// SETUP for SSL:
    /// 
    /// STEP 1: install the server certificate, and update the server

    /// STEP 2: Map the SSL cert to the selfhost port, e.g. you need to get the cert hash for your server certificate
    ///     netsh http add sslcert ipport=0.0.0.0:50231 certhash=507146b0b51032d812045fcdf2beacc1eaec620c appid={DAEFA3B4-8827-47B3-9981-004E63F5DA59}

    /// CLEAN up for SSL
    ///    netsh http delete sslcert ipport=0.0.0.0:50231
    ///    
    /// Please update the ServerCertSubjectName with the subject name of your server certificate and ClientCertSubjectName for your client certificate.
    /// If you client certificate not located in the CurrentUser/Personal store then you can update the GetClientCertificate() method to return 
    /// the client certificate your app uses.
    /// 
    /// </summary>
    class Program
    {
        static readonly Uri _baseAddress = new Uri("https://localhost:50231/");

        // Update here with your client and server certificate subject name
        public const string ServerCertSubjectName = "service.com";
        public const string ClientCertSubjectName = "HongMei Ge";
        public const string ClientCertHash = "858CBC654C3B31C7CEEEF71A737A9CAD245FD52E";

        static void Main(string[] args)
        {
            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
            config.HostNameComparisonMode = HostNameComparisonMode.Exact;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // tell the system that the server requires client cert
            config.ClientCredentialType = HttpClientCredentialType.Certificate;

            // turn off the built in certificate authentication
            config.X509CertificateValidator = new NoOpValidator();

            // set up the message handler to convert the client certificate to principal with Administrator role
            config.MessageHandlers.Add(new CustomCertificateMessageHandler());

            HttpSelfHostServer server = null;
            try
            {
                // create the server 
                server = new HttpSelfHostServer(config);

                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress + "...\n");

                // RunClient
                RunClient();
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

        private static void RunClient()
        {
            // Perform the Server Certificate validation
            ServicePointManager.ServerCertificateValidationCallback += Program.RemoteCertificateValidationCallback;

            // start the client
            WebRequestHandler handler = new WebRequestHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual; // this would pick from the Current user store
            handler.ClientCertificates.Add(GetClientCertificate());
            
            HttpClient client = new HttpClient(handler);
            client.BaseAddress = _baseAddress;
            HttpResponseMessage response;

            // How to post a sample item with success
            SampleItem sampleItem = new SampleItem { Id = 1, StringValue = "hello from a new sample item" };
            response = client.PostAsJsonAsync("/api/sample/", sampleItem).Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Posting the first item returns:\n" + response.Content.ReadAsStringAsync().Result + "\n");
            response.Dispose();

            // How to get all the sample Items, we should see the newly added sample item that we just posted
            response = client.GetAsync("/api/sample/").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Getting all the items returns:\n" + response.Content.ReadAsStringAsync().Result + "\n");
            response.Dispose();

            // How to post another sample item with success
            sampleItem = new SampleItem { Id = 2, StringValue = "hello from a second new sample item" };
            response = client.PostAsJsonAsync("/api/sample/", sampleItem).Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Posting a second item returns:\n" + response.Content.ReadAsStringAsync().Result + "\n");
            response.Dispose();

            // How to get all the sample Items, we should see the two newly added sample item that we just posted
            response = client.GetAsync("/api/sample/").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Getting all the items returns:\n" + response.Content.ReadAsStringAsync().Result + "\n");
            response.Dispose();

            // How to get an sample item with id = 2
            response = client.GetAsync("/api/sample/2").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("Getting the second item returns:\n" + response.Content.ReadAsStringAsync().Result + "\n");
            response.Dispose();
        }

        public static bool RemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return certificate.Subject.EndsWith(Program.ServerCertSubjectName);
        }

        // You may need to update here to if you client certificate locates in other x509 store
        private static X509Certificate GetClientCertificate()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509CertificateCollection col = store.Certificates.Find(X509FindType.FindBySubjectName, ClientCertSubjectName, true);
            return col[1];
        }

        /// <summary>
        /// This class disables the default certificate validation, we are doing the client certificate checking in the CustomCertificateMessageHandler
        /// </summary>
        class NoOpValidator : X509CertificateValidator
        {
            public override void Validate(X509Certificate2 certificate)
            {
               return;
            }
        }
    }
}
