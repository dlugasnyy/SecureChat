using System.Collections.Concurrent;

namespace WebSocketServer;

public class ConnectionHandler
{
    private ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();
    
    public ConnectionHandler()
    {
        
    }

    public void AddNewSocketConnection(WebSocket socket)
    {
        //retry jakis?
        
        _connections.TryAdd(CreateSocketId(), socket);
    }

    public async Task CloseConnection(WebSocket webSocket)
    {
        //TODO:  cancelationtoken, retry of removing socket and check for null
        var socketToClose = _connections.FirstOrDefault(c => c.Value == webSocket).Key; // osobna metoda
        _connections.TryRemove(socketToClose, out var socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed connection", CancellationToken.None);
    }
        
    private string CreateSocketId()
    {
        return Guid.NewGuid().ToString();
    }
    
    
}