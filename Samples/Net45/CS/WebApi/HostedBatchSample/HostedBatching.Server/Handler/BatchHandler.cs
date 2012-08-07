using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace HostedBatching
{
    /// <summary>
    /// BatchHandler servers as the endpoint handling all batching requests. It is registered to 
    /// the route "api/batch". Requests received by BatchHandler will not be sent to the dispatcher.
    /// Instead BatchHandler reads the body part requests and redirect them to in-memory
    /// BatchServer. These requests then go through the pipeline and get back to the BatchHandler.
    /// The response messages are eventually added to one batch response as HttpMessageContent.
    /// </summary>
    public class BatchHandler : DelegatingHandler
    {
        private HttpMessageInvoker _entry;

        public BatchHandler(HttpConfiguration configuration)
        {
            _entry = new HttpMessageInvoker(new BatchServer(configuration));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content == null ||
                !request.Content.IsMimeMultipartContent("batch"))
            {
                return request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The mediatype is expected to be \"multipart/batch\" for a batching request");
            }

            MultipartContent outerContent = new MultipartContent("batch");
            HttpResponseMessage outerResponse = request.CreateResponse();
            outerResponse.Content = outerContent;

            MultipartMemoryStreamProvider multipart = await request.Content.ReadAsMultipartAsync();

            /// Load requests from multipart content. Submit each of them 
            /// to the in memory batch server sequentially. They are then
            /// processed asynchronously.
            /// The individual response are gathered and added to the 
            /// multipart content for return response.
            foreach (var content in multipart.Contents)
            {
                HttpResponseMessage innerResponse = null;

                try
                {
                    var innerRequest = await content.ReadAsHttpRequestMessageAsync();

                    innerResponse = await _entry.SendAsync(innerRequest, cancellationToken);
                }
                catch (Exception)
                {
                    innerResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                outerContent.Add(new HttpMessageContent(innerResponse));
            }

            return outerResponse;
        }
    }
}
