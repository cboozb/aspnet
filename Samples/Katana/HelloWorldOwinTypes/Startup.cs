using Owin;
using Owin.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelloWorldOwinTypes
{

    // Note the Web.Config owin:HandleAllRequests setting that is used to direct all requests to your OWIN application.
    // Alternatively you can specify routes in the global.asax file.
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