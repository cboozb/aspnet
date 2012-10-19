using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// This sample client reads a never-ending HTTP response sent from the
    /// <see cref="PushContentController"/> and prints the output to the console.
    /// </summary>
    /// <remarks>This sample is written using Tasks for .NET 4 but would 
    /// benefit greatly from using the async and await keywords in .NET 4.5</remarks>
    class Program
    {
        static readonly Uri _address = new Uri("http://localhost:50232/api/pushcontent");

        static void RunClient()
        {
            HttpClient client = new HttpClient();

            // Issue GET request 
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = _address,
            };

            client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ContinueWith(
                (getTask) =>
                {
                    if (getTask.IsCanceled)
                    {
                        return;
                    }
                    if (getTask.IsFaulted)
                    {
                        throw getTask.Exception;
                    }
                    HttpResponseMessage response = getTask.Result;

                    // Get response stream
                    response.Content.ReadAsStreamAsync().ContinueWith(
                        (streamTask) =>
                        {
                            if (streamTask.IsCanceled)
                            {
                                return;
                            }
                            if (streamTask.IsFaulted)
                            {
                                throw streamTask.Exception;
                            }

                            // Read response stream
                            byte[] readBuffer = new byte[512];
                            ReadResponseStream(streamTask.Result, readBuffer);
                        });
                });
        }

        private static void ReadResponseStream(Stream rspStream, byte[] readBuffer)
        {
            Task.Factory.FromAsync<byte[], int, int, int>(rspStream.BeginRead, rspStream.EndRead, readBuffer, 0, readBuffer.Length, state: null).ContinueWith(
                (readTask) =>
                {
                    if (readTask.IsCanceled)
                    {
                        return;
                    }
                    if (readTask.IsFaulted)
                    {
                        throw readTask.Exception;
                    }

                    int bytesRead = readTask.Result;
                    string content = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                    Console.WriteLine("Received: {0}", content);

                    if (bytesRead != 0)
                    {
                        ReadResponseStream(rspStream, readBuffer);
                    }
                });
        }

        static void Main(string[] args)
        {
            RunClient();

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
    }
}
