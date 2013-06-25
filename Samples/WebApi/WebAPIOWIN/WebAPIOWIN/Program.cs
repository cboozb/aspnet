using System;
using System.Net.Http;
using Microsoft.Owin.Hosting;

namespace WebAPIOWIN
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = string.Format("http://localhost:1025");

            //Start OWIN host
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                //Create HttpCient and make a request to api/values
                HttpClient client = new HttpClient();

                var response = client.GetAsync(baseAddress + "/api/values").Result;

                Console.WriteLine(response.Content.ReadAsStringAsync().Result);

                Console.ReadLine();
            }
        }
    }
}
