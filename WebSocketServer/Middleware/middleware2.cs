namespace WebSocketServer.Middleware;

public class Middleware2
{
    private readonly RequestDelegate _next;
    private readonly ConnectionHandler _handler;

    public Middleware2(RequestDelegate next, ConnectionHandler handler)
    {
        _next = next;
        _handler = handler;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
            return;
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Debug.WriteLine("Websocket connected");
        
        _handler.AddNewSocketConnection(webSocket);//zwrocic id?
        
        var cancellationToken = context.RequestAborted;

        await ReceiveMessage(webSocket, cancellationToken, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                //duzo rzeczy
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await _handler.CloseConnection(webSocket);
            }
        });
    }

    private async Task ReceiveMessage(WebSocket socket, CancellationToken cancellationToken,
        Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), cancellationToken);
            handleMessage(result, buffer);
        }
    }
}