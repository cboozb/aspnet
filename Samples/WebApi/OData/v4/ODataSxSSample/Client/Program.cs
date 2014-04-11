using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient() { BaseAddress = new Uri("http://localhost:12374/") };

            // Access the V1 service without any version query string
            var response = client.GetAsync("/odata/Orders/").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nOrder list from the V1 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V1 service with the V1 version query string
            response = client.GetAsync("/odata/Products(1)/?v=1").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nProduct list from the V1 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V1 metadata without any version query string
            response = client.GetAsync("/odata/$metadata").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nMetadata from the V1 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V1 metadata with the V1 version query string
            response = client.GetAsync("/odata/$metadata?v=1").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nMetadata from the V1 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V2 service with the V2 version query string. The orders service in V2 uses the OData Attribute Routing when selecting actions. 
            response = client.GetAsync("/odata/Orders(1)/?v=2").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nOrder list from the V2 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V2 service with the V2 version query string. The products service in V2 rely on the traditional convention when selecting actions. 
            response = client.GetAsync("/odata/Products(1)/?v=2").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nProduct list from the V2 service returned: " + response.Content.ReadAsStringAsync().Result);

            // Access the V2 metadata with the V2 version version query string
            response = client.GetAsync("/odata/$metadata?v=2").Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("\nMetadata from the V2 service returned: " + response.Content.ReadAsStringAsync().Result);

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
