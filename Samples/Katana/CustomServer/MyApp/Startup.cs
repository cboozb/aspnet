using Owin;
using Owin.Types;
using System.Threading.Tasks;

namespace MyApp
{
    public class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder builder)
        {
            builder.UseHandlerAsync(Invoke);
        }

        // Invoked once per request.
        public Task Invoke(OwinRequest request, OwinResponse response)
        {
            response.ContentType = "text/plain";
            return response.WriteAsync("Hello World");
        }
    }
}
