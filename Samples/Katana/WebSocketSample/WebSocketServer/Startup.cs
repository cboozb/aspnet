
using Owin;
using Owin.Types;
using System;
using System.Threading.Tasks;

namespace WebSocketServer
{
    /// <summary>
    /// This sample requires Windows 8, .NET 4.5, and Microsoft.Owin.Host.HttpListener.
    /// </summary>
    public class Startup
    {
        // Run at startup
        public void Configuration(IAppBuilder builder)
        {
            builder.UseHandlerAsync(UpgradeToWebSockets);
        }

        // Run once per request
        private Task UpgradeToWebSockets(OwinRequest request, OwinResponse response, Func<Task> next)
        {
            if (!request.CanAccept)
            {
                // Not a websocket request
                return next();
            }

            request.Accept(WebSocketEcho);

            return Task.FromResult<object>(null);
        }

        private async Task WebSocketEcho(OwinWebSocket websocket)
        {
            byte[] buffer = new byte[1024];
            OwinWebSocketReceiveMessage received = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), websocket.CallCancelled);
            
            while (websocket.ClientCloseStatus == 0)
            {
                // Echo anything we receive
                await websocket.SendAsync(new ArraySegment<byte>(buffer, 0, received.Count), received.MessageType, received.EndOfMessage, websocket.CallCancelled);

                received = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), websocket.CallCancelled);
            }

            await websocket.CloseAsync(websocket.ClientCloseStatus, websocket.ClientCloseDescription, websocket.CallCancelled);
        }
    }
}