using System;
using System.Net.Http;

namespace ProductODataClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RunClient("odata/Products/ProductODataService.Models.GetCount(Name='%2F')");
            RunClient("odata/Products/ProductODataService.Models.GetCount(Name='Name2%5C')");
            RunClient("odata/Products/ProductODataService.Models.GetCount(Name='Name2')");
            RunClient("odata/Products?$filter=endswith(Name,'%5C')");
        }

        static void RunClient(string odataPath)
        {
            const string baseAddress = "http://localhost:46769/";

            // Create HttpCient and make a request to api/values 
            var client = new HttpClient();

            var response =
                client.GetAsync(baseAddress + odataPath).Result;

            Console.WriteLine(response);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }
    }
}
