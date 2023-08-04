// Copr. (c) Nexus 2023. All rights reserved.

using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;

namespace Openlabs.Mgcxm.Net;

public sealed class MgcxmSocket : IMgcxmSystemObject
{
    public static MgcxmSocket Create(HttpListenerWebSocketContext context)
    {
        return new MgcxmSocket
        {
            _socketId = Hashing.MD5(context.WebSocket.GetHashCode().ToString()),
            _context = context
        };
    }

    public async Task SendMessage(byte[] data)
    {
        await _context.WebSocket.SendAsync(
            data, 
            WebSocketMessageType.Text, 
            true, 
            CancellationToken.None);
    }

    public async Task SendMessage(string data)
        => await SendMessage(Encoding.UTF8.GetBytes(data));

    public async Task SendMessage<T>(T data)
        => await SendMessage(JsonConvert.SerializeObject(data));

    public WebSocket GetRawWebSocket() => _context.WebSocket;

    public string GetSocketId() => _socketId;

    private string _socketId;
    private HttpListenerWebSocketContext _context;
    
    #region IMgcxmSystemObject
    public MgcxmId AllocatedId { get; }

    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);
    #endregion
}