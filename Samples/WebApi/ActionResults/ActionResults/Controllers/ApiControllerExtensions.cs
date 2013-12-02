using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;
using ActionResults.Results;

namespace ActionResults.Controllers
{
    static class ApiControllerExtensions
    {
        public static OkTextPlainResult Text(this ApiController controller, string content)
        {
            return Text(controller, content, Encoding.UTF8);
        }

        public static OkTextPlainResult Text(this ApiController controller, string content, Encoding encoding)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            return new OkTextPlainResult(content, encoding, controller.Request);
        }

        public static OkFileDownloadResult Download(this ApiController controller, string path, string contentType)
        {
            string downloadFileName = Path.GetFileName(path);
            return Download(controller, path, contentType, downloadFileName);
        }

        public static OkFileDownloadResult Download(this ApiController controller, string path, string contentType, string downloadFileName)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            return new OkFileDownloadResult(MapPath(path), contentType, downloadFileName, controller.Request);
        }

        private static string MapPath(string path)
        {
            // The following code is for demonstration purposes only and is not fully robust for production usage.
            // HttpContext.Current is not always available after asynchronous calls complete.
            // Also, this call is host-specific and will need to be modified for other hosts such as OWIN.
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}