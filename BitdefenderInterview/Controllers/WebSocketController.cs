using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace BitdefenderInterview.Controllers
{
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        [HttpGet("connect")]
        public async Task<IActionResult> Connect()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketCommunication(webSocket);
                return new EmptyResult();
            }
            else
            {
                return BadRequest("WebSocket request expected.");
            }
        }

        private static async Task HandleWebSocketCommunication(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received: {message}");

                var responseMessage = $"Server received: {message}";
                var responseBuffer = Encoding.UTF8.GetBytes(responseMessage);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
