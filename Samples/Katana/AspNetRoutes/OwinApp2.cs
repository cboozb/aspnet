using Owin.Types;
using System.Threading.Tasks;

namespace AspNetRoutes
{
    public class OwinApp2
    {
        // Invoked once per request.
        public static Task Invoke(OwinRequest request, OwinResponse response)
        {
            response.ContentType = "text/plain";
            return response.WriteAsync("Hello World 2");
        }
    }
}