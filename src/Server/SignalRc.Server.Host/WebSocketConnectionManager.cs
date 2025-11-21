using System.Net.WebSockets;
using System.Text;
using SignalRc.Server.Host.Feature;

namespace SignalRc.Server.Host;
public class WebSocketConnectionManager
{
    private readonly List<WebSocket> _sockets = new();
    private readonly IMessageHandler _messageHandler;
    private readonly ILogger<WebSocketConnectionManager> _logger;

    public WebSocketConnectionManager(IMessageHandler messageHandler, ILogger<WebSocketConnectionManager> logger)
    {
        _messageHandler = messageHandler;
        _logger = logger;
    }

    public async Task HandleConnection(WebSocket socket)
    {
        _sockets.Add(socket);

        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            try
            {
                await _messageHandler.Handle(message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString(), e);
            }

            // Broadcast to all clients
            await BroadcastAsync(message);
        }

        _sockets.Remove(socket);
        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
    }

    public async Task BroadcastAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);

        foreach (var socket in _sockets.ToList())
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}