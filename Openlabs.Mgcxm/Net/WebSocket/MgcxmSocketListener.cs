// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Transactions;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net.Polling;
using Random = Openlabs.Mgcxm.Internal.Random;

namespace Openlabs.Mgcxm.Net;

/*public class MgcxmSocketListenerInherited : MgcxmHttpListener
{
    public MgcxmSocketListenerInherited(IpAddress addressToHostOn) : base(addressToHostOn)
    {
    }
}*/

public class MgcxmSocketListener : IMgcxmSystemObject
{
    public MgcxmSocketListener(IpAddress addressToHostOn)
    {
        _listener = new MgcxmHttpListener(addressToHostOn);
        _pollingReceiver.OnPollReceived.AddAction((pollRequest) => { });
        
        AllocatedId = GetHashCode();
        MgcxmObjectManager.Register(AllocatedId, this); // register object
        AttributeResolver.ResolveAllMethod<SocketRouteAttribute>(
            GetType(),
            typeof(SocketRouteAttribute),
            (method, attr) =>
            {
                _routes.Add(new MgcxmSocketRoute(AllocatedId, attr.Route, false, 
                    (socket) =>
                    {
                        var parameters = method.GetParameters();
                        bool isInvalidSig = parameters.Count() != 1 || (parameters[0].ParameterType != typeof(MgcxmSocket));

                        if (isInvalidSig) return; // cannot add endpoints with invalid signatures
                        method.Invoke(this, new object[] { socket });
                    }, method));
                
                Logger.Trace(string.Format("Added '0x{0:x8}' to '0x{1:x8}'", attr.Route.GetHashCode(),
                    AllocatedId.Id));
            });
    }
    ~MgcxmSocketListener() => Trash();

    public void Start()
    {
        _listener.StartInternal(HttpServerListenTask);
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private bool IsValidRoute(string routeUrl, out MgcxmSocketRoute foundRoute)
    {
        foundRoute = null;
        
        foreach (var route in _routes)
        {
            if (route.Route == routeUrl)
            {
                foundRoute = route;
                return true;
            }
        }

        return false;
    }

    private async Task HttpServerListenTask(HttpListener listener)
    {
        listener.Start();

        while (_listener.ListenToRequests)
        {
            var context = await listener.GetContextAsync();
            string routeUrl = context.Request.RawUrl.Contains("?") 
                ? context.Request.RawUrl.Split("?")[0] 
                : context.Request.RawUrl;
            
            // accept ws request
            if (context != null &&
                IsValidRoute(routeUrl, out var route) && 
                context.Request.IsWebSocketRequest)
            {
                var socketContext = await context.AcceptWebSocketAsync(null);
            
                Logger.Info($"||==----------- Ws Request -----------==||");
                Logger.Info($"Server Id = 0x{AllocatedId.Id:x8}");
                Logger.Info($"Requested Url = {routeUrl}");

                (new Thread(async () =>
                {
                    var socket = MgcxmSocket.Create(socketContext);
                    string socketId = socket.GetSocketId();
                    MgcxmString socketId_str = socketId;
                    
                    if (!_socketThreads.ContainsKey(socketId_str)) 
                        _socketThreads.Add(socketId_str, new SocketThread(_pollingReceiver));
                    
                    _pollingReceiver.SendPoll(this, status => { });
                    await _socketThreads[socketId_str].Start(socket, route);
                    
                    // once its done, remove the socket
                    if (_socketThreads.ContainsKey(socketId_str)) _socketThreads.Remove(socketId_str);
                })).Start();
            }

            // accept any non-ws requests
            if (context != null && !context.Request.IsWebSocketRequest)
            {
                var httpRequest = context.Request;
                var httpResponse = context.Response;
                
                // build needed data
                string query = "";
                string url = httpRequest.Url.PathAndQuery;

                Uri baseAddress = new Uri($"{httpRequest.Url.Scheme}://{httpRequest.Url.Host}:{httpRequest.Url.Port}");
                HttpMethods method = HttpMethods.UNKNOWN;
                Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
                Dictionary<string, string> queryData = new Dictionary<string, string>();
                Dictionary<string, string> formData = new Dictionary<string, string>();
                List<byte> bodyData = new List<byte>();

                Dictionary<MgcxmString, MgcxmString> responseHeaders = new Dictionary<MgcxmString, MgcxmString>();

                bool canParseMethod = Enum.TryParse(typeof(HttpMethods), httpRequest.HttpMethod, out object? methodObj);
                if (canParseMethod && methodObj != null)
                    method = (HttpMethods)methodObj;

                for (int i = 0; i < httpRequest.Headers.Count; i++)
                    requestHeaders.Add(
                        httpRequest.Headers.GetKey(i) ?? "Unknown",
                        httpRequest.Headers.GetValues(i)?[0] ?? "Unknown");

                if (url.Contains("?"))
                {
                    var urlParts = url.Split("?");

                    url = WebUtility.UrlDecode(urlParts[0]);
                    query = urlParts[1] ?? "";

                    foreach (var queryPart in query.Split("&"))
                    {
                        var kvp = queryPart.Split("=");
                        if (kvp.Length > 1)
                            queryData.Add(kvp[0], kvp[1]);
                    }
                }

                int ibyte;
                while ((ibyte = httpRequest.InputStream.ReadByte()) != -1)
                    bodyData.Add((byte)ibyte);

                for (int i = 0; i < httpResponse.Headers.Count; i++)
                    responseHeaders.Add(
                        httpResponse.Headers.GetKey(i),
                        httpResponse.Headers.GetValues(i)?[0] ?? "Unknown");

                // construct data
                var requestData = MgcxmHttpRequest.New(
                    method,
                    baseAddress,
                    url,
                    HttpContentTypeHelper.ResolveValue(httpRequest.ContentType),
                    bodyData.ToArray(),
                    formData,
                    requestHeaders,
                    queryData,
                    AllocatedId);

                var responseData = MgcxmHttpResponse.New(
                    (HttpStatusCodes)httpResponse.StatusCode,
                    "",
                    responseHeaders,
                    Array.Empty<byte>(),
                    AllocatedId);

                // log request
                Logger.Info($"||==----------- Ws Http Request -----------==||");
                Logger.Info($"Server Id = 0x{AllocatedId.Id:x8}");
                Logger.Info($"Requested Url = {requestData.Uri}");
                Logger.Info($"Http Method = {requestData.HttpMethod}");
                if (requestData.HttpMethod == HttpMethods.POST || requestData.HttpMethod == HttpMethods.PUT)
                {
                    Logger.Info($"Content Type = {requestData.ContentType}");
                    Logger.Info($"Content Length = {requestData.RawBodyData.Length}");
                    Logger.Info($"Content = {requestData.Body.Truncate(15, "...")}");
                }

                // add required headers
                responseData.Header("X-Powered-By", "Mgcxm.Net");
                responseData.Header("Vary", "Accept-Encoding");
                responseData.Header("Request-Context", $"appId=cid-v1:{Guid.NewGuid().ToString()}");
                responseData.Header("Pragma", "no-cache");
                responseData.Header("Cache-Control", "no-cache");
                responseData.Header("Expires", "-1");
            
                httpResponse.KeepAlive = true;
                responseData.Header("Connection", "keep-alive");
                
                OnWebRequest(requestData, responseData);
                await responseData.Transfer(httpResponse);
            }
        }
        
        listener.Stop();
    }
    
    public virtual void OnWebRequest(MgcxmHttpRequest request, MgcxmHttpResponse response) {}

    #region IMgcxmSystemObject

    public MgcxmId AllocatedId { get; }

    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);

    #endregion
    
    public MgcxmHttpListener Listener => _listener;

    private MgcxmHttpListener _listener;
    private List<MgcxmSocketRoute> _routes = new();
    private PollingReceiver _pollingReceiver = new();

    private Thread _listenThread;
    private Dictionary<MgcxmString, SocketThread> _socketThreads = new();

    private class SocketThread
    {
        public SocketThread(PollingReceiver receiver)
        {
            _pollingReceiver = receiver;
            _pollingReceiver.OnPollReceived.AddAction(OnPollRequest);
        }

        private void OnPollRequest(PollRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            if (_socket == null) request.Accept(-1);
            else
            {
                var socket = _socket.GetRawWebSocket();
                request.Accept((int)socket.State);
            }
        }

        public async Task Start(MgcxmSocket socket, MgcxmSocketRoute route)
        {
            _route = route ?? throw new ArgumentNullException(nameof(route));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));

            SocketConnectionHandlerAttribute attr = null;
            if ((attr = route.MethodInfo.GetCustomAttribute<SocketConnectionHandlerAttribute>()) != null)
                _connectionHandler = (MgcxmSocketConnectionHandler)Activator.CreateInstance(attr.HandlerType);

            WebSocketCloseStatus? closeStatus = null;
            bool keepListening = true;
            if (_connectionHandler != null) _connectionHandler.Internal_OnConnection(socket);
            while (keepListening)
            {
                _webSocket = _socket.GetRawWebSocket();

                byte[] buffer = new byte[16 * 16 * 4 * 2 * 2];
                var messageResult = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

                if ((closeStatus = messageResult.CloseStatus) != null)
                    keepListening = false; // clear execution path
                else
                {
                    byte[] data = new byte[messageResult.Count];
                    for (int i = 0; i < messageResult.Count; i++)
                        data[i] = buffer[i];
                    if (_connectionHandler != null) _connectionHandler.OnMessage(data);
                }
            }
            if (_connectionHandler != null) _connectionHandler.OnDisconnection(closeStatus);
        }

        private MgcxmSocketConnectionHandler _connectionHandler;
        private MgcxmSocketRoute _route;
        private MgcxmSocket _socket;
        private WebSocket _webSocket;
        private PollingReceiver _pollingReceiver;
    }
}