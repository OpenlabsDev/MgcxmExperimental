// Copr. (c) Nexus 2023. All rights reserved.

using System.Net.WebSockets;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an abstract base class for handling WebSocket connections and events.
/// </summary>
public abstract class MgcxmSocketConnectionHandler
{
    /// <summary>
    /// Sets the internal MgcxmSocket object when a connection is established.
    /// </summary>
    /// <param name="socket">The MgcxmSocket representing the connected socket.</param>
    internal void Internal_OnConnection(MgcxmSocket socket)
    {
        Socket = socket;
    }

    /// <summary>
    /// Invoked when a WebSocket connection is established.
    /// </summary>
    /// <param name="socket">The MgcxmSocket representing the connected socket.</param>
    public abstract void OnConnection(MgcxmSocket socket);

    /// <summary>
    /// Invoked when a message is received over the WebSocket connection.
    /// </summary>
    /// <param name="data">The byte array representing the received message data.</param>
    public abstract void OnMessage(byte[] data);

    /// <summary>
    /// Invoked when the WebSocket connection is closed.
    /// </summary>
    /// <param name="status">The close status of the WebSocket connection, if available.</param>
    public abstract void OnDisconnection(WebSocketCloseStatus? status);

    /// <summary>
    /// Gets the MgcxmSocket representing the current WebSocket connection.
    /// </summary>
    public MgcxmSocket Socket { get; private set; }
}