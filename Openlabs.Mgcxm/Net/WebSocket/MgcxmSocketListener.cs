// Copr. (c) Nexus 2023. All rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
    private Dictionary<string, SocketThread> _socketThreads = new();

    public static int ServerCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the MgcxmSocketListener class with the specified IP address to host on.
    /// </summary>
    /// <param name="addressToHostOn">The IP address to host the socket listener on.</param>
    /// <param name="certificate">The SSL certificate to use.</param>
    public MgcxmSocketListener(IpAddress addressToHostOn, string host = "localhost", string postfix = "/", X509Certificate2 certificate = null)
    {
        ServerCount++;

        GCWrapper.SuppressFinalize(this);
        GCWrapper.SuppressFinalize(_routes);
        GCWrapper.SuppressFinalize(AllocatedId);
        GCWrapper.SuppressFinalize(_socketThreads);

        _listener = new MgcxmHttpListener(addressToHostOn, host, postfix, certificate);

        AllocatedId = GetHashCode();
        MgcxmObjectManager.Register(AllocatedId, this); // register object
        AttributeResolver.ResolveAllMethod<SocketRouteAttribute>(
            GetType(),
            typeof(SocketRouteAttribute),
            (method, attr) =>
            {
                _routes.Add(new MgcxmSocketRoute(AllocatedId, attr.Route.TrimEnd('/'), false,
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
    ~MgcxmSocketListener()
    {
        ServerCount--;
        Trash();
    }

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
            if (route.Route == routeUrl.TrimEnd('/'))
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

            var validRoute = IsValidRoute(requestData.Uri, out var route);
            if (validRoute) Logger.Trace("Valid route found: " + requestData.Uri);
            if (context.Request.IsWebSocketRequest)
                Logger.Trace("Is websocket request");

            // accept ws request
            if (context != null &&
                validRoute &&
                context.Request.IsWebSocketRequest)
            {
                var socketContext = await context.AcceptWebSocketAsync(null);

                Logger.Info($"--------------- Ws Request ---------------");
                Logger.Info($"Server Id = 0x{AllocatedId.Id:x8} ({this.GetType().Name})");
                Logger.Info($"Requested Url = {requestData.Uri}");

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Factory.StartNew(async () =>
                {
                    Events.OnWsRequestMade.Invoke(this, route);

                    var socket = MgcxmSocket.Create(socketContext);
                    string socketId = socket.GetSocketId();
                    route.OnSocketUpgraded(socket);

                    try
                    {
                        SocketThread socketThread = null;
                        if (!_socketThreads.ContainsKey(socketId))
                            _socketThreads.Add(socketId, socketThread = new SocketThread());
                        else socketThread = _socketThreads[socketId];

                        await socketThread.Start(socket, route, requestData);

                        // once its done, remove the socket
                        _socketThreads.Remove(socketId);
                    }
                    catch (Exception ex)
                    {
                        Logger.Exception("Failed to serve WS request", ex);
                        if (_socketThreads.ContainsKey(socketId))
                            _socketThreads.Remove(socketId);
                    }
                }, TaskCreationOptions.LongRunning);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            // accept any non-ws requests
            else if (context != null && !context.Request.IsWebSocketRequest)
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

                await OnWebRequest(requestData, responseData);
                await responseData.Transfer(httpResponse);

                if (MgcxmConfiguration.HasBootstrapConfiguration && MgcxmConfiguration.CurrentBootstrapConfiguration.logRequests)
                {
                    File.AppendAllText("httpLog.log", string.Format("---------- HTTP Request ----------\nUrl: {0}\nMethod: {1}\nContent-Type: {2}\nPost Data: {3}\nResponse: {4}\n",
                        requestData.Uri,
                        requestData.HttpMethod,
                        requestData.ContentType,
                        requestData.Body,
                        Encoding.UTF8.GetString(responseData.ResponseData)));
                }
            }
            else
                Logger.Error("Failed to accept websocket request. Context = " + (context != null ? string.Format("0x{0:x2}", context.GetHashCode()) : "null"));
        }
        
        listener.Stop();
    }

    /// <summary>
    /// Invoked when a WebSocket request is received.
    /// </summary>
    /// <param name="request">The HTTP request data.</param>
    /// <param name="response">The HTTP response data.</param>
    public virtual async Task OnWebRequest(MgcxmHttpRequest request, MgcxmHttpResponse response) 
    {
        response.Status(HttpStatusCodes.NotFound).Finish();
    }

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
        public SocketThread()
        {
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
            Stopwatch sw = new Stopwatch();

            if (!_connectionHandler.NegotiateUpgrade(negotiateUpgradeRequest))
            {
                Logger.Error("Websocket failed authentication.");
                return;
            }

            if (_connectionHandler != null) _connectionHandler.Internal_OnConnection(socket);
            while (keepListening)
            {
                try
                {
                    if (!keepListening) // clear execution path for late update of keepListening
                        return;

                    _webSocket = _socket.GetRawWebSocket();
                    byte[] buffer = new byte[16 * 16 * 4 * 2 * 2];

                    if (_webSocket.State == WebSocketState.Closed ||
                        _webSocket.State == WebSocketState.CloseSent ||
                        _webSocket.State == WebSocketState.CloseReceived ||
                        _webSocket.State == WebSocketState.Aborted)
                    {
                        keepListening = false;
                        Logger.Trace("Tried receiving a message when the state was Closed. This is never supposed to happen.");
                        return;
                    }

                    Logger.Trace(string.Format("Receiving message (starting stopwatch, id = {0}, current state = {1})", _socket.GetSocketId(), _webSocket.State));

                    sw.Restart();
                    var messageResult = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    sw.Stop();

                    Logger.Trace(string.Format("Received message ({0} byte(s), {1} ms to receive)", messageResult.Count, sw.ElapsedMilliseconds));

                    if ((closeStatus = messageResult.CloseStatus) != null)
                        keepListening = false; // clear execution path
                    else
                    {
                        byte[] data = new byte[messageResult.Count];
                        for (int i = 0; i < messageResult.Count; i++)
                            data[i] = buffer[i];

                        if (MgcxmConfiguration.HasBootstrapConfiguration && MgcxmConfiguration.CurrentBootstrapConfiguration.logRequests)
                        {
                            File.AppendAllText("httpLog.log", string.Format("---------- WS Request (Client To Server) ----------\nRoute: {0}\nMessage: {1}\n",
                                _route.Route,
                                Encoding.UTF8.GetString(data)));
                        }

                        if (_connectionHandler != null)
                            _connectionHandler.OnMessage(data);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception("Failed to receive WS data", ex);
                }
            }
            if (_connectionHandler != null) _connectionHandler.OnDisconnection(closeStatus);
        }

        private MgcxmSocketConnectionHandler _connectionHandler;
        private MgcxmSocketRoute _route;
        private MgcxmSocket _socket;
        private WebSocket _webSocket;
    }
}