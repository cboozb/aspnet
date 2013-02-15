using Owin.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
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

            string responseText = request.GetHeader("breadcrumbs") + "\r\n"
                + "PathBase: " + request.PathBase + "\r\n"
                + "Path: " + request.Path + "\r\n";
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

            response.SetHeader("Content-Length", responseBytes.Length.ToString(CultureInfo.InvariantCulture));
            response.SetHeader("Content-Type", "text/plain");

            Stream responseStream = response.Body;
            return Task.Factory.FromAsync(responseStream.BeginWrite, responseStream.EndWrite, responseBytes, 0, responseBytes.Length, null);
            // 4.5: return responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
    }
}