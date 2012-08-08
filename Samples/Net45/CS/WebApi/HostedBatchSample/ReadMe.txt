HostedBatchSample
-----------------

This sample shows how to implement HTTP batching within ASP.NET. The batching consists
of putting multiple HTTP requests within a single MIME multipart entity body which
is then sent to the server as an HTTP POST. The requests are then processed
individually and the responses are put into another MIME multipart entity body
which is returned to the client.

To run the sample, build the project and deploy HostedBatching server. The 
default deployment configuration is IIS Express. And then compile and run 
the HostedBatching.TestClient. The client makes request to http://localhost:29135, 
which is the default Project URI if you don't change the server configuration. If 
you decide to use something other than IIS Express or use different URI, you need to 
update the test client.

For a detailed description of this sample, please see
http://trocolate.wordpress.com/2012/07/19/mitigate-issue-260-in-batching-scenario/

This sample is provided as part of the ASP.NET Web Stack sample repository at
http://aspnet.codeplex.com/

For more information about the samples, please see
http://go.microsoft.com/fwlink/?LinkId=261487