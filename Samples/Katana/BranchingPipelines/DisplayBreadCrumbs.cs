using Owin.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BranchingPipelines
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class DisplayBreadCrumbs
    {
        public DisplayBreadCrumbs(AppFunc ignored)
        {
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            OwinRequest request = new OwinRequest(environment);
            OwinResponse response = new OwinResponse(environment);

            response.ContentType = "text/plain";

            string responseText = request.GetHeader("breadcrumbs") + "\r\n"
                + "PathBase: " + request.PathBase + "\r\n"
                + "Path: " + request.Path + "\r\n";

            return response.WriteAsync(responseText);
        }
    }
}