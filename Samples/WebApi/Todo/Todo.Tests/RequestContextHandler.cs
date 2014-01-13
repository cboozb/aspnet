using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Todo.Tests
{
    public class RequestContextHandler : DelegatingHandler
    {
        public IPrincipal Principal { get; set; }
        public IOwinContext OwinContext { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpRequestContext context = request.GetRequestContext();
            context.Principal = Principal;
            request.SetOwinContext(this.OwinContext);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
