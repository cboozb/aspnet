using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Todo.Tests.Helpers
{
    public class NonDisposableRequestHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!(request is NonDisposableRequest))
            {
                request = new NonDisposableRequest(request);
            }
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            NonDisposableRequest nonDisposableRequest = response.RequestMessage as NonDisposableRequest;
            if (nonDisposableRequest != null)
            {
                response.RequestMessage = nonDisposableRequest.GetDisposableRequest();
            }
            return response;
        }
    }
}
