using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;
using Owin;

namespace ODataOpenTypeSample
{
    public class Program
    {
        private static readonly string _baseAddress = string.Format("http://{0}:12345",System.Environment.MachineName);
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _namespace = typeof(Account).Namespace;

        public static void Main(string[] args)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            using (WebApp.Start(_baseAddress, Configuration))
            {
                Console.WriteLine("Listening on " + _baseAddress);

                QueryAccount();
                QueryAddressFromAccount();
                AddAccount();
                PutAccount();
                DeleteAccount();

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        public static void Configuration(IAppBuilder builder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapODataServiceRoute(routeName: "OData", routePrefix: "odata", model: ODataModels.GetModel());
            builder.UseWebApi(config);
        }

        public static void QueryMetadata()
        {
            string requestUri = _baseAddress + "/odata/$metadata";
            using (HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                string metadata = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("\nThe metadata is \n {0}", metadata);
            }
        }

        public static void AddAccount()
        {
            string requestUri = _baseAddress + "/odata/Accounts";

            JObject postContent = JObject.Parse(@"
                    {'Id':1,
                    'Name':'Ben',
                    'Address':{'City':'Shanghai','Street':'Zixing','Country':'China'},
                    }");// Country is a dynamic property
            using (HttpResponseMessage response = _httpClient.PostAsJsonAsync(requestUri, postContent).Result)
            {
                response.EnsureSuccessStatusCode();
                JObject result = response.Content.ReadAsAsync<JObject>().Result;
                Console.WriteLine("\nThe newly added account is:");
                Console.WriteLine(result);
            }
        }

        public static void PutAccount()
        {
            string requestUri = _baseAddress + "/odata/Accounts(1)";

            JObject postContent = JObject.Parse(@"
                    {'Id':1,
                    'Name':'Jinfu',
                    'Address':{'City':'Beijing','Street':'Changan','Emails':'a@a.com,b@b.com'},
                    }");// Emails is a dynamic property
            using (HttpResponseMessage response = _httpClient.PutAsJsonAsync(requestUri, postContent).Result)
            {
                response.EnsureSuccessStatusCode();
                JObject result = response.Content.ReadAsAsync<JObject>().Result;
                Console.WriteLine("\nThe updated account is:");
                Console.WriteLine(result);
            }
        }

        public static void DeleteAccount()
        {
            string requestUri = _baseAddress + "/odata/Accounts(1)";

            using (HttpResponseMessage response = _httpClient.DeleteAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                JObject result = response.Content.ReadAsAsync<JObject>().Result;
                Console.WriteLine("\nThe Accounts(1) is deleted.");
                Console.WriteLine(result);
            }
        }

        public static void QueryAccount()
        {
            string requestUri = _baseAddress + "/odata/Accounts(1)";

            using (HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                JObject result = response.Content.ReadAsAsync<JObject>().Result;
                Console.WriteLine("\nThe Accounts(1) is :");
                Console.WriteLine(result);
            }
        }

        public static void QueryAddressFromAccount()
        {
            string requestUri = _baseAddress + "/odata/Accounts(1)/Address";

            using (HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result)
            {
                response.EnsureSuccessStatusCode();
                JObject result = response.Content.ReadAsAsync<JObject>().Result;
                Console.WriteLine("\nThe address of Accounts(1) is :");
                Console.WriteLine(result);
            }
        }
    }
}
