// Copr. (c) Nexus 2023. All rights reserved.

using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Tests;

public sealed class TestWsServer : MgcxmSocketListener
{
    public TestWsServer() : base("localhost:8011") { }

    public override void OnWebRequest(MgcxmHttpRequest request, MgcxmHttpResponse response)
    {
        response.Content(HttpContentTypes.PlainText, "Cannot access this URL");
    }

    [SocketRoute("/api/notification/v2"), SocketConnectionHandler(typeof(NotificationSocketConnectionHandler))]
    public void OnSocketUpgraded_Notification(MgcxmSocket socket) { }

    public class NotificationSocketConnectionHandler : MgcxmSocketConnectionHandler
    {
        public override void OnConnection(MgcxmSocket socket)
        {
            Logger.Debug("Client connect");
        }

        public override void OnMessage(byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            Logger.Debug("Client message: " + message);

            Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            if (json.ContainsKey("PlayerId") &&
                json.ContainsKey("AppVersion") &&
                json.ContainsKey("IpAddress") &&
                json.ContainsKey("VRDevice") &&
                json.ContainsKey("IsDevelopment"))
            {
                var clientMessage = JsonConvert.DeserializeObject<ClientConnectMessage>(message);
                this.Socket.SendMessage(new ClientConnectAckMessage
                { SessionId = 1 });
            }
            else
            {
                
            }
        }

        public override void OnDisconnection(WebSocketCloseStatus? status)
        {
            Logger.Debug("Client disconnect");
        }
    }
    
    public class ClientConnectMessage
    {
        public string PlayerId { get; set; }
        public string AppVersion { get; set; }
        public string IpAddress { get; set; }
        public string VRDevice { get; set; }
        public string IsDevelopment { get; set; }
    }

    public class ClientConnectAckMessage
    {
        public long SessionId { get; set; }
    }
}