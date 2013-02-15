using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StaticFiles
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using SendFileFunc = Func<string, long, long?, CancellationToken, Task>;

    public class ServeSpecificPage
    {
        private AppFunc _next;
        private string _filePath;

        public ServeSpecificPage(AppFunc next, string filePath)
        {
            _next = next;
            _filePath = filePath;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            object obj;
            if (!environment.TryGetValue("sendfile.SendAsync", out obj))
            {
                throw new PlatformNotSupportedException("SendFile is not supported by this server");
            }
            SendFileFunc sendFile = (SendFileFunc)obj;
            return sendFile(_filePath, 0, null, (CancellationToken)environment["owin.CallCancelled"]);
        }
    }
}
