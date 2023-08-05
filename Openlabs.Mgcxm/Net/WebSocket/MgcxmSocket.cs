// Copr. (c) Nexus 2023. All rights reserved.

using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents a managed socket for handling WebSocket communication using HttpListenerWebSocketContext.
/// </summary>
public sealed class MgcxmSocket : IMgcxmSystemObject
{
    private string _socketId;
    private HttpListenerWebSocketContext _context;

    /// <summary>
    /// Creates a new instance of MgcxmSocket from the provided HttpListenerWebSocketContext.
    /// </summary>
    /// <param name="context">The HttpListenerWebSocketContext used to initialize the MgcxmSocket.</param>
    /// <returns>A new instance of MgcxmSocket.</returns>
    public static MgcxmSocket Create(HttpListenerWebSocketContext context)
    {
        return new MgcxmSocket
        {
            _socketId = Hashing.MD5(context.WebSocket.GetHashCode().ToString()),
            _context = context
        };
    }

    /// <summary>
    /// Sends a binary message over the WebSocket connection.
    /// </summary>
    /// <param name="data">The byte array to be sent as a message.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendMessage(byte[] data)
    {
        await _context.WebSocket.SendAsync(
            data,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    /// <summary>
    /// Sends a string message over the WebSocket connection.
    /// </summary>
    /// <param name="data">The string data to be sent as a message.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendMessage(string data)
        => await SendMessage(Encoding.UTF8.GetBytes(data));

    /// <summary>
    /// Sends a serialized object as a JSON message over the WebSocket connection.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized and sent.</typeparam>
    /// <param name="data">The object to be serialized and sent.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendMessage<T>(T data)
        => await SendMessage(JsonConvert.SerializeObject(data));

    /// <summary>
    /// Retrieves the underlying raw WebSocket associated with the MgcxmSocket.
    /// </summary>
    /// <returns>The raw WebSocket object.</returns>
    public WebSocket GetRawWebSocket() => _context.WebSocket;

    /// <summary>
    /// Retrieves the unique identifier (socket ID) associated with the MgcxmSocket.
    /// </summary>
    /// <returns>The socket ID.</returns>
    public string GetSocketId() => _socketId;

    #region IMgcxmSystemObject

    /// <summary>
    /// Gets the unique identifier (object ID) of the MgcxmSocket.
    /// </summary>
    public MgcxmId AllocatedId { get; }

    /// <summary>
    /// Disposes of the MgcxmSocket and deregisters it from the object manager.
    /// </summary>
    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);

    #endregion
}