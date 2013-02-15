using System.Linq;
using System.Web.Http;

namespace HostedBatching
{
    public class BatchServer : HttpServer
    {
        private readonly HttpConfiguration _config;

        public BatchServer(HttpConfiguration configuration)
            : base(configuration)
        {
            _config = configuration;
        }

        /// <summary>
        /// Override the default Initialize to prevent pipeline from being re-wired.
        /// </summary>
        protected override void Initialize()
        {
            var firstInPipeline = _config.MessageHandlers.FirstOrDefault();
            if (firstInPipeline != null && firstInPipeline.InnerHandler != null)
            {
                InnerHandler = firstInPipeline;
            }
            else
            {
                base.Initialize();
            }
        }
    }
}