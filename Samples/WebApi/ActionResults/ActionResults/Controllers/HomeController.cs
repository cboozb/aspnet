using System.Text;
using System.Web.Http;

namespace ActionResults.Controllers
{
    public class HomeController : ApiController
    {
        [Route("text")]
        public IHttpActionResult GetText()
        {
            return this.Text("Hello, world!");
        }

        [Route("text_ascii")]
        public IHttpActionResult GetTextAscii()
        {
            return this.Text("Hello, world!", Encoding.ASCII);
        }

        [Route("file")]
        public IHttpActionResult GetFile()
        {
            return this.Download("Download.xml", "application/xml");
        }
    }
}