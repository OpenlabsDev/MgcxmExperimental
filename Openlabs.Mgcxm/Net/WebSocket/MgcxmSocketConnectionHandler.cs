// Copr. (c) Nexus 2023. All rights reserved.

using System.Net.WebSockets;

namespace Openlabs.Mgcxm.Net;

public abstract class MgcxmSocketConnectionHandler
{
    internal void Internal_OnConnection(MgcxmSocket socket)
    {
        Socket = socket;
    }
    
    public abstract void OnConnection(MgcxmSocket socket);
    public abstract void OnMessage(byte[] data);
    public abstract void OnDisconnection(WebSocketCloseStatus? status);
    
    public MgcxmSocket Socket { get; private set; }
}