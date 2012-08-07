using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMessageHandlerPipelineSample
{
    internal class SampleHandler : DelegatingHandler
    {
        private string _text;
        private string _indentation;

        public SampleHandler(string text, int indent)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (indent < 0)
            {
                throw new ArgumentException("indent");
            }
            _text = text;
            _indentation = new String(' ', indent);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("{0}{1} request path", _indentation, _text);
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    Console.WriteLine("{0}{1} response path", _indentation, _text);
                    return task.Result;
                });
        }
    }
}
