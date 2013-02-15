using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HostedBatching.selfhost
{
    /// <summary>
    /// A console application demonstrating batching scenario from client actions.
    /// </summary>
    class Program
    {
        static void Main()
        {
            Task test = RunTest("http://localhost:29135");
            test.Wait();

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        private static async Task RunTest(string baseAddress)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage batchRequest = new HttpRequestMessage(
                HttpMethod.Post,
                baseAddress + "/api/batch"
            );

            MultipartContent batchContent = new MultipartContent("batch");
            batchRequest.Content = batchContent;

            CreateBatchedRequest(baseAddress, batchContent);

            using (var stdout = Console.OpenStandardOutput())
            {
                Console.WriteLine("<<< REQUEST >>>");
                Console.WriteLine();
                Console.WriteLine(batchRequest);
                Console.WriteLine();
                await batchContent.CopyToAsync(stdout);
                Console.WriteLine();

                var batchResponse = await client.SendAsync(batchRequest);

                Console.WriteLine("<<< RESPONSE >>>");
                Console.WriteLine();
                Console.WriteLine(batchResponse);
                Console.WriteLine();
                await batchResponse.Content.CopyToAsync(stdout);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Create a batching request content. The content contains three body part requests.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="batchContent"></param>
        private static void CreateBatchedRequest(string baseAddress, MultipartContent batchContent)
        {
            batchContent.Add(
                new HttpMessageContent(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        baseAddress + "/api/demo"
                    )
                )
            );

            // this request is expected to recieve a 404
            batchContent.Add(
                new HttpMessageContent(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        baseAddress + "/not/found"
                    )
                )
            );

            batchContent.Add(
                new HttpMessageContent(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        baseAddress + "/api/demo/1"
                    )
                )
            );
        }
    }
}