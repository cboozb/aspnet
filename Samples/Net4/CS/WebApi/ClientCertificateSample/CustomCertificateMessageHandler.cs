using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace ClientCertificateSample
{
    /// <summary>
    /// This verifies the client certificate is one of those known certificates and creates an generic principals with "Administrators" role. 
    /// </summary>
    public class CustomCertificateMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            X509Certificate cert = request.GetClientCertificate();
            if (cert != null)
            {
                if (cert.GetCertHashString() == Program.ClientCertHash)
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(cert.Subject), new[] { "Administrators" });
                }
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
