using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionResults.Results
{
    public class OkFileDownloadResult : IHttpActionResult
    {
        public OkFileDownloadResult(string localPath, string contentType, string downloadFileName,
            HttpRequestMessage request)
        {
            if (localPath == null)
            {
                throw new ArgumentNullException("localPath");
            }

            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            if (downloadFileName == null)
            {
                throw new ArgumentNullException("downloadFileName");
            }

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            LocalPath = localPath;
            ContentType = contentType;
            DownloadFileName = downloadFileName;
            Request = request;
        }

        public string LocalPath { get; private set; }

        public string ContentType { get; private set; }

        public string DownloadFileName { get; private set; }

        public HttpRequestMessage Request { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(File.OpenRead(LocalPath));
            response.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = DownloadFileName
            };
            return response;
        }
    }
}