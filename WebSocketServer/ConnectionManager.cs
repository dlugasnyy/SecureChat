using System.Collections.Concurrent;

namespace WebSocketServer;

public class ConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> _sockets;
    public ConcurrentDictionary<string, WebSocket> GetAllSockets()
    {
        return _sockets;
    }

    public bool CheckIfUserAlreadyExists(string connId)
    {
        return _sockets.ContainsKey(connId);
    }

    public string AddSocket(WebSocket socket)
    {
        var connId = Guid.NewGuid().ToString();
        
        _sockets.TryAdd(connId, socket);
        Debug.WriteLine($"connection added: {connId}");
        return connId;
    }
}