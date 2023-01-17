using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace WebSocketServer.Middleware;

public class WebSocketServerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly WebSocketServerConnectionManager _manager;

    public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager)
    {
        _next = next;
        _manager = manager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //Console.WriteLine("WebSocket Connected");
            Debug.WriteLine("Websocket connected");
            var connId = _manager.AddSocket(webSocket);
            await SendConnIdAsync(webSocket, connId); 
            await ReceiveMessage(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    Console.WriteLine($"Receive->Text");
                    Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                    await RouteJsonMessage(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    string id = _manager.GetAllSockets().FirstOrDefault(s => s.Value == webSocket).Key;
                    Console.WriteLine($"Receive->Close on: " + id);

                    WebSocket sock;
                    _manager.GetAllSockets().TryRemove(id, out sock);
                    Console.WriteLine("Managed Connections: " + _manager.GetAllSockets().Count.ToString());

                    await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                    return;
                }
            });
        }
        else
        {
            Debug.WriteLine("Hello from 2nd delegate;");
            await _next(context);
        }
    }

    private async Task SendConnIdAsync(WebSocket socket, string connId)
    {
        var buffer = Encoding.UTF8.GetBytes($"connId: {connId}");
        await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }
    
    private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);
            handleMessage(result, buffer);
        }
    }

    public async Task RouteJsonMessage(string message)
    {
        var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
        Console.WriteLine("To: " + routeOb.To);
        Guid guidOutput;

        if (Guid.TryParse(routeOb.To.ToString(), out guidOutput))
        {
            Console.WriteLine("Targeted");
            var sock = _manager.GetAllSockets().FirstOrDefault(s => s.Key == routeOb.To.ToString());
            if (sock.Value != null)
            {
                if (sock.Value.State == WebSocketState.Open)
                    await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Console.WriteLine("Invalid Recipient");
            }
        }
        else
        {
            Console.WriteLine("Broadcast");
            foreach (var sock in _manager.GetAllSockets())
            {
                if (sock.Value.State == WebSocketState.Open)
                    await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
    
    
}