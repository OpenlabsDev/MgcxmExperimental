// Copr. (c) Nexus 2023. All rights reserved.

using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net.Polling;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Net;

[Obsolete("Do not use. This was a test that has been scrapped.", error: true)]
/// <summary>
/// Represents a socket listener for handling WebSocket and HTTP requests.
/// </summary>
/// <remarks>
/// This class uses the <see cref="MgcxmHttpListener"/> as a base class.
/// <para>There are supposed to be bugs, because this is a test class.</para>
/// </remarks>
internal class MgcxmSocketListenerInherited : MgcxmHttpListener
{
    public MgcxmSocketListenerInherited(IpAddress addressToHostOn) : base(addressToHostOn)
    {
    }

    public void Start()
    {
        base.StartInternal(ListenToRequests);
    }

    private async Task ListenToRequests(HttpListener listener)
    {
        listener.Start();

        while (base.ListenToRequests)
        {
        }

        listener.Stop();
    }
}

/// <summary>
/// Represents a socket listener for handling WebSocket and HTTP requests.
/// </summary>
public class MgcxmSocketListener : IMgcxmSystemObject, IStartableServer
{
    private MgcxmHttpListener _listener;
    private List<MgcxmSocketRoute> _routes = new();
    private PollingReceiver _pollingReceiver = new();
    private Dictionary<MgcxmString, SocketThread> _socketThreads = new();

    /// <summary>
    /// Initializes a new instance of the MgcxmSocketListener class with the specified IP address to host on.
    /// </summary>
    /// <param name="addressToHostOn">The IP address to host the socket listener on.</param>
    /// <param name="certificate">The SSL certificate to use.</param>
    public MgcxmSocketListener(IpAddress addressToHostOn, string host = "localhost", string postfix = "/", X509Certificate2 certificate = null)
    {
        GCWrapper.SuppressFinalize(this);
        GCWrapper.SuppressFinalize(_routes);
        GCWrapper.SuppressFinalize(AllocatedId);
        GCWrapper.SuppressFinalize(_pollingReceiver);
        GCWrapper.SuppressFinalize(_socketThreads);

        _listener = new MgcxmHttpListener(addressToHostOn, host, postfix, certificate);
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

    /// <summary>
    /// Finalizer (destructor) for the MgcxmSocketListener class.
    /// </summary>
    ~MgcxmSocketListener() => Trash();

    /// <summary>
    /// Starts the socket listener.
    /// </summary>
    public void Start()
    {
        _listener.StartInternal(HttpServerListenTask);
    }

    /// <summary>
    /// Stops the socket listener.
    /// </summary>
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
            MgcxmHttpListener.FormatUrl(ref routeUrl);

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            // build needed data
            string query = "";
            string url = httpRequest.Url.PathAndQuery;

            MgcxmHttpListener.CreateWebRequestData(AllocatedId, ref url, ref query, httpRequest, httpResponse, out MgcxmHttpRequest requestData, out MgcxmHttpResponse responseData);

            // add required headers
            responseData.Header("X-Powered-By", "Mgcxm.Net");
            responseData.Header("Vary", "Accept-Encoding");
            responseData.Header("Request-Context", $"appId=cid-v1:{Guid.NewGuid().ToString()}");
            responseData.Header("Pragma", "no-cache");
            responseData.Header("Cache-Control", "no-cache");
            responseData.Header("Expires", "-1");

            httpResponse.KeepAlive = true;
            responseData.Header("Connection", "keep-alive");

            // accept ws request
            if (context != null &&
                IsValidRoute(routeUrl, out var route) && 
                context.Request.IsWebSocketRequest)
            {
                var socketContext = await context.AcceptWebSocketAsync(null);
            
                Logger.Info($"--------------- Ws Request ---------------");
                Logger.Info($"Server Id = 0x{AllocatedId.Id:x8} ({this.GetType().Name})");
                Logger.Info($"Requested Url = {routeUrl}");

                (new Thread(async () =>
                {
                    var socket = MgcxmSocket.Create(socketContext);
                    string socketId = socket.GetSocketId();
                    MgcxmString socketId_str = socketId;
                    
                    if (!_socketThreads.ContainsKey(socketId_str)) 
                        _socketThreads.Add(socketId_str, new SocketThread(_pollingReceiver));
                    
                    _pollingReceiver.SendPoll(this, status => { });
                    await _socketThreads[socketId_str].Start(socket, route, requestData);
                    
                    // once its done, remove the socket
                    if (_socketThreads.ContainsKey(socketId_str)) _socketThreads.Remove(socketId_str);
                })).Start();
            }

            // accept any non-ws requests
            if (context != null && !context.Request.IsWebSocketRequest)
            {

                // log request
                Logger.Info($"--------------- Ws Http Request ---------------");
                Logger.Info($"Server Id = 0x{AllocatedId.Id:x8} ({this.GetType().Name})");
                Logger.Info($"Requested Url = {requestData.Uri}");
                Logger.Info($"Http Method = {requestData.HttpMethod}");
                if (requestData.HttpMethod == HttpMethods.POST || requestData.HttpMethod == HttpMethods.PUT)
                {
                    Logger.Info($"Content Type = {requestData.ContentType}");
                    Logger.Info($"Content Length = {requestData.RawBodyData.Length}");
                    Logger.Info($"Content = {requestData.Body.Truncate(15, "...")}");
                }

                OnWebRequest(requestData, responseData);
                await responseData.Transfer(httpResponse);
            }
        }
        
        listener.Stop();
    }

    /// <summary>
    /// Invoked when a WebSocket request is received.
    /// </summary>
    /// <param name="request">The HTTP request data.</param>
    /// <param name="response">The HTTP response data.</param>
    public virtual void OnWebRequest(MgcxmHttpRequest request, MgcxmHttpResponse response) { }

    #region IMgcxmSystemObject

    /// <summary>
    /// Gets the unique identifier (object ID) of the MgcxmSocketListener.
    /// </summary>
    public MgcxmId AllocatedId { get; }

    /// <summary>
    /// Disposes of the MgcxmSocketListener and deregisters it from the object manager.
    /// </summary>
    public void Trash() => MgcxmObjectManager.Deregister(AllocatedId);

    #endregion

    /// <summary>
    /// Gets the underlying MgcxmHttpListener used by the MgcxmSocketListener.
    /// </summary>
    public MgcxmHttpListener Listener => _listener;

    public X509Certificate2 Certificate => Listener.Certificate;
    public IpAddress Address => Listener.Address;

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

        public async Task Start(MgcxmSocket socket, MgcxmSocketRoute route, MgcxmHttpRequest negotiateUpgradeRequest)
        {
            _route = route ?? throw new ArgumentNullException(nameof(route));
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));

            SocketConnectionHandlerAttribute attr = null;
            if ((attr = route.MethodInfo.GetCustomAttribute<SocketConnectionHandlerAttribute>()) != null)
                _connectionHandler = (MgcxmSocketConnectionHandler)Activator.CreateInstance(attr.HandlerType);

            WebSocketCloseStatus? closeStatus = null;
            bool keepListening = true;

            if (!_connectionHandler.NegotiateUpgrade(negotiateUpgradeRequest)) return;
            if (_connectionHandler != null) _connectionHandler.Internal_OnConnection(socket);
            while (keepListening)
            {
                try
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
                catch { }
            }
            if (_connectionHandler != null) _connectionHandler.OnDisconnection(closeStatus);
            _pollingReceiver.OnPollReceived.RemoveAction(OnPollRequest);
        }

        private MgcxmSocketConnectionHandler _connectionHandler;
        private MgcxmSocketRoute _route;
        private MgcxmSocket _socket;
        private WebSocket _webSocket;
        private PollingReceiver _pollingReceiver;
    }
}