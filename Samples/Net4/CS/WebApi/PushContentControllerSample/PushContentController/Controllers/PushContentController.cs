using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace PushContentController.Controllers
{
    /// <summary>
    /// This controller pushes data to the client as a never-ending HTTP response writing a little
    /// bit of data every 1 second. There can be N number of clients connected at any given time -- the
    /// list of clients is maintained in a concurrent dictionary. Clients are removed if writes fail
    /// which means that they have disconnected.
    /// </summary>
    /// <remarks>
    /// The sample needs to run using either full IIS/ASP or IIS Express. The Visual Studio Development
    /// Server does not work.
    /// </remarks>
    public class PushContentController : ApiController
    {
        private static readonly Lazy<Timer> _timer = new Lazy<Timer>(() => new Timer(TimerCallback, null, 0, 1000));
        private static readonly ConcurrentDictionary<StreamWriter, StreamWriter> _outputs = new ConcurrentDictionary<StreamWriter, StreamWriter>();

        public HttpResponseMessage GetUpdates(HttpRequestMessage request)
        {
            Timer t = _timer.Value;
            request.Headers.AcceptEncoding.Clear();
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(OnStreamAvailable, "text/plain");
            return response;
        }

        private static void OnStreamAvailable(Stream stream, HttpContent headers, TransportContext context)
        {
            StreamWriter sWriter = new StreamWriter(stream);
            _outputs.TryAdd(sWriter, sWriter);
        }

        private static void TimerCallback(object state)
        {
            foreach (var kvp in _outputs.ToArray())
            {
                try
                {
                    kvp.Value.Write(DateTime.Now);
                    kvp.Value.Flush();
                }
                catch
                {
                    StreamWriter sWriter;
                    _outputs.TryRemove(kvp.Value, out sWriter);
                }
            }
        }
    }
}