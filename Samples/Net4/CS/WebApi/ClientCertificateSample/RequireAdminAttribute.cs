using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ClientCertificateSample
{
    /// <summary>
    /// This authorize attribute would fail if the client certificate is not in the role of Administrators.
    /// </summary>
    public class RequireAdminAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            // do authorization based on the principal.
            IPrincipal principal = Thread.CurrentPrincipal;
            if (principal == null || !principal.IsInRole("Administrators"))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }
}
