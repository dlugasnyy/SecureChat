using System.Collections.Concurrent;

namespace WebSocketServer;

public class WebSocketServerConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
    public ConcurrentDictionary<string, WebSocket> GetAllSockets()
    {
        return _sockets;
    }

    public string AddSocket(WebSocket socket)
    {
        var ConnId = Guid.NewGuid().ToString();
        _sockets.TryAdd(ConnId, socket);
        Debug.WriteLine($"connection added: {ConnId}");
        return ConnId;
    }
}