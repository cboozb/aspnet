using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HostedBatching.Handler
{
    /// <summary>
    /// This message handler is used to demo how the handlers other than batch handler 
    /// be handled in pipeline.
    /// </summary>
    public class SampleMessageHandler : DelegatingHandler
    {
        private string _tag;

        public SampleMessageHandler(string tag)
        {
            _tag = tag;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response != null)
            {
                response.Headers.Add("SampleHeaderName", "SampleHeaderValue");
            }

            return response;
        }
    }
}