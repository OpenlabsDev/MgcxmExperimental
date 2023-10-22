// Copr. (c) Nexus 2023. All rights reserved.

using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;
using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Common.Framework;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net.Extensions;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Provides utility methods related to IP addresses.
/// </summary>
public static class AddressUtilities
{
    /// <summary>
    /// Formats the IP address. If the address is "localhost", it returns "+".
    /// </summary>
    /// <param name="addr">The IP address to format.</param>
    /// <returns>The formatted IP address.</returns>
    public static string FormatIpAddress(string addr)
    {
        if (addr == "localhost") return "+";
        return addr;
    }
}

/// <summary>
/// Represents an HTTP listener for MgcxmSocketListener.
/// </summary>
public class MgcxmHttpListener : IStartableServer
{
    private string _host;
    public Action<MgcxmHttpRequest, MgcxmHttpResponse> OnWebRequest;

    /// <summary>
    /// Initializes a new instance of the MgcxmHttpListener class.
    /// </summary>
    /// <param name="addressToHostOn">The IP address to host the listener on.</param>
    /// <param name="host">The host to use for the server. Default value is localhost</param>
    /// <param name="postfix">The path to use on the server. Default value is /</param>
    /// <param name="certificate">The SSL certificate to use.</param>
    public MgcxmHttpListener(IpAddress addressToHostOn, string host = "localhost", string postfix = "/", X509Certificate2 certificate = null)
    {
        GCWrapper.SuppressFinalize(this);
        GCWrapper.SuppressFinalize(_allocatedId);

        _host = host;
        Certificate = certificate;
        Address = addressToHostOn;

        string scheme = "http"; // default scheme is http
        if (certificate != null) scheme += "s";

        // Initialize the HttpListener with the provided IP address and port.
        _listener = new HttpListener();
        _listener.Prefixes.Add(
            $"{scheme}://{AddressUtilities.FormatIpAddress(addressToHostOn.Origin)}:{addressToHostOn.Port}{postfix}");

        // Generate an allocated ID for the listener.
        _allocatedId = GetHashCode();

        _endpoints = new List<MgcxmHttpEndpoint>();

        // Create core endpoints, e.g., /favicon.ico.
        InitializeCore();

        // Register the listener object.
        MgcxmObjectManager.Register(_allocatedId, this);

        // Resolve and add all endpoints with the HttpEndpointAttribute.
        AttributeResolver.ResolveAllMethod<HttpEndpointAttribute>(
            GetType(),
            typeof(HttpEndpointAttribute),
            (method, attr) =>
            {
                // Check if an endpoint with the same URL already exists.
                if (_endpoints.Any(x => x.Url == attr.Url))
                    return; // Cannot add endpoints with the same URL.

                var dynamicUseAttribute = method.GetCustomAttribute<HttpDynamicUseAttribute>();

                // Split the URL and find dynamic identifiers, if any.
                string[] urlParts = attr.Url.Split("/");
                List<MgcxmDynamicIdentifier> currentIdentifiers = new List<MgcxmDynamicIdentifier>();
                for (int i = 0; i < urlParts.Length; i++)
                {
                    if (dynamicUseAttribute != null && dynamicUseAttribute.DynamicIdentifiers.Contains(urlParts[i]))
                    {
                        currentIdentifiers.Add(
                            new MgcxmDynamicIdentifier(
                                i,
                                dynamicUseAttribute.DynamicIdentifiers.First(x => x == urlParts[i])));
                    }
                }

                // Set the return type for the attribute.
                attr.ReturnType = method.ReturnType;

                // Create a new endpoint and add it to the list.
                _endpoints.Add(new MgcxmHttpEndpoint(
                    _allocatedId, attr.Url, currentIdentifiers.ToArray(), attr.HttpMethod,
                    (req, res) =>
                    {
                        var parameters = method.GetParameters();
                        bool isInvalidSig = parameters.Count() != 2 ||
                                            (parameters[0].ParameterType != typeof(MgcxmHttpRequest) &&
                                             parameters[1].ParameterType != typeof(MgcxmHttpResponse));

                        if (isInvalidSig) return; // Cannot add endpoints with invalid signatures
                        method.Invoke(this, new object[] { req, res });
                    }));

                Logger.Trace(string.Format("Added '0x{0:x8}' to '0x{1:x8}'", attr.Url.GetHashCode(),
                    _allocatedId.Id));
            });
        if (_endpoints != null) GCWrapper.SuppressFinalize(_endpoints);
    }

    /// <summary>
    /// Finalizes an instance of the MgcxmHttpListener class.
    /// </summary>
    ~MgcxmHttpListener() => MgcxmObjectManager.Deregister(_allocatedId);

    /// <summary>
    /// Initializes core endpoints, e.g., /favicon.ico and other systems.
    /// </summary>
    private void InitializeCore()
    {
        const long FAVICON_ID = 2441798;

        Logger.Debug($"Creating identifier 0x{FAVICON_ID:x8} pointing 0x{_allocatedId.Id:x8}");

        // Create /favicon.ico page uri pointing to 0x00254246
        _endpoints.Add(new MgcxmHttpEndpoint(FAVICON_ID, "/favicon.ico", Array.Empty<MgcxmDynamicIdentifier>(), HttpMethods.GET,
            (request, response) =>
            {
                response.Status(HttpStatusCodes.OK).Content(HttpContentTypes.BinaryData, Array.Empty<byte>()).Finish();
            }));
    }

    private async Task StartListening()
    {
        _listener.Start();
        _listenToRequests = true;

        while (_listenToRequests)
        {
            var httpContext = await _listener.GetContextAsync();

            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;

#pragma warning disable
            Task.Run(async () =>
            {
                // build needed data
                string query = "";
                string url = httpRequest.Url!.PathAndQuery;

                CreateWebRequestData(_allocatedId, ref url, ref query, httpRequest, httpResponse, out MgcxmHttpRequest requestData, out MgcxmHttpResponse responseData);
                var requestId = Uuid.Create();

                try
                {

                    //Logger.Info($"=========== Ws Request ===========");
                    //Logger.Info($"==================================");

                    // log request
                    Logger.Info($"--------------- Http Request ---------------");
                    Logger.Info($"Server Id = 0x{_allocatedId.Id:x8} ({this.GetType().Name})");
                    Logger.Info($"Requested Url = {requestData.Uri}");
                    Logger.Info($"Http Method = {requestData.HttpMethod}");
                    if (requestData.HttpMethod == HttpMethods.POST || requestData.HttpMethod == HttpMethods.PUT)
                    {
                        Logger.Info($"Content Type = {requestData.ContentType}");
                        Logger.Info($"Content Length = {requestData.RawBodyData.Length}");
                        Logger.Info($"Content = {requestData.Body.Truncate(50, "...")}");
                    }

                    // add required headers
                    responseData.Header("X-Powered-By", "Mgcxm.Net");
                    responseData.Header("Vary", "Accept-Encoding");
                    responseData.Header("Request-Context", $"appId=cid-v1:{Guid.NewGuid().ToString()}");
                    responseData.Header("Pragma", "no-cache");
                    responseData.Header("Cache-Control", "no-cache");
                    responseData.Header("Expires", "-1");
                    responseData.Header("Connection", "keep-alive");

                    if (httpRequest.Url.Host != _host)
                    {
                        responseData.Status(0)
                                    .Content(HttpContentTypes.HtmlFile, "<h1>Invalid host</h1>")
                                    .Finish();
                        await responseData.Transfer(httpResponse);
                        return;
                    }

                    bool foundEndpointDebounce = false;

                    // write to response
                    if (_framework == null && _endpoints.Count > 1)
                    {
                        Logger.Trace("Using default handling");
                        var endpoint = _endpoints.Find(x => x.Url == requestData.Uri && x.RequiredMethod == Enum.Parse<HttpMethods>(httpRequest.HttpMethod));
                        if (endpoint != null)
                        {
                            foundEndpointDebounce = true;
                            endpoint.OnEndpointRequested(requestData, responseData);

                            await TaskEx.WaitUntil(() => responseData.FinishedBuilding);
                            await responseData.Transfer(httpResponse);
                        }
                        else
                        {
                            // try using the Matches method
                            foreach (var vEndpoint in _endpoints)
                            {
                                try
                                {
                                    if (MgcxmHttpEndpoint.Matches(requestData.Uri, vEndpoint, requestData))
                                    {
                                        foundEndpointDebounce = true;
                                        // Logger.Trace($"0x{vEndpoint.EndpointId.Id:x8} meets dynamic criterion");
                                        vEndpoint.OnEndpointRequested(requestData, responseData);

                                        var sw = Stopwatch.StartNew();
                                        await TaskEx.WaitUntil(() => responseData.FinishedBuilding, 25, 5500);
                                        if (sw.ElapsedMilliseconds > 5000)
                                        {
                                            responseData.Status(HttpStatusCodes.BadGateway)
                                                        .Content(HttpContentTypes.PlainText, "Task ex failed to wait")
                                                        .Finish();
                                            await responseData.Transfer(httpResponse);
                                        }
                                        else
                                            await responseData.Transfer(httpResponse);
                                    }
                                    else
                                    {
                                        // Logger.Trace($"0x{vEndpoint.EndpointId.Id:x8} does not meet dynamic criterion");
                                    }
                                }
                                catch (Exception exception)
                                {
                                    // Logger.Trace($"0x{vEndpoint.EndpointId.Id:x8} does not meet dynamic criterion");
                                    Logger.Exception("Cannot parse Dynamic Url", exception);
                                }
                            }
                        }
                    }
                    else if (_framework != null && _framework.Endpoints.Count > 0)
                    {
                        Logger.Trace(string.Format("Using framework handling. Framework = {0}, Endpoints = {1}",
                            _framework == null ? "null" : "not null",
                            _framework != null ? _framework.Endpoints.Count : 0));
                        try
                        {
                            await _framework.ResolveRequest(httpRequest, httpResponse, requestData, responseData);
                        }
                        catch (Exception exception)
                        {
                            Logger.Exception("Cannot resolve Framework endpoint", exception);
                        }
                    }
                    else
                    {
                        // THIS IS A LEGACY OPTION!
                        // THIS MAY NOT ALWAYS WORK

                        // ==========================================================================
                        // 6:00 PM (10/18/2023)
                        // Author: nexus
                        //
                        // this option is known to break every so often, please dont
                        // use it lol
                        // ==========================================================================
                        Logger.Trace("Using legacy request handling: OnWebRequest()");

                        this.OnWebRequest.Invoke(requestData, responseData);

                        var sw = Stopwatch.StartNew();
                        await TaskEx.WaitUntil(() => responseData.FinishedBuilding, 25, 5500);
                        if (sw.ElapsedMilliseconds > 5000)
                        {
                            responseData.Status(HttpStatusCodes.BadGateway)
                                        .Content(HttpContentTypes.PlainText, "Task ex failed to wait")
                                        .Finish();
                            await responseData.Transfer(httpResponse);
                        }
                        else
                            await responseData.Transfer(httpResponse);
                    }

                    if (!foundEndpointDebounce)
                    {
                        responseData.Status(HttpStatusCodes.NotFound)
                                    .Content(HttpContentTypes.PlainText, "The URL was not found on this server.")
                                    .Finish();

                        await responseData.Transfer(httpResponse);
                        foundEndpointDebounce = true;
                    }

                    if (MgcxmConfiguration.HasBootstrapConfiguration && MgcxmConfiguration.CurrentBootstrapConfiguration.logRequests)
                    {
                        File.AppendAllText("httpLog.log", string.Format("---------- HTTP Request ----------\nUrl: {0}\nMethod: {1}\nContent-Type: {2}\nPost Data: {3}\nResponse: {3}\n",
                            requestData.Uri,
                            requestData.HttpMethod,
                            requestData.ContentType,
                            requestData.Body,
                            Encoding.UTF8.GetString(responseData.ResponseData)));
                    }

                }
                catch (Exception ex)
                {
                    string[] html = new string[]
                    {
                        "<!DOCTYPE html>",
                        "<html>",
                        "   <body>",
                        "       <h1>Internal Server Error</h1>",
                        $"       <h3>An error occurred while processing your request. (Request ID: {requestId.ToString()})</h3>",
                        "       <hr>",
                        $"       <strong>{ex.ToString()}</strong>",
                        "   </body>",
                        "</html>"
                    };
                    responseData.Status(HttpStatusCodes.InternalServerError)
                                .Content(HttpContentTypes.HtmlFile, string.Join("\n", html))
                                .Finish();

                    await responseData.Transfer(httpResponse);
                }
            });
#pragma warning enable
        }

        _listener.Stop();
    }

    public static void CreateWebRequestData(MgcxmId allocatedId, ref string url, ref string query, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse, out MgcxmHttpRequest requestData, out MgcxmHttpResponse responseData)
    {
        // trim any extra / from the start
        url = url.TrimStart('/');
        url = string.Format("/{0}", url);

        Uri baseAddress = new Uri($"{httpRequest.Url.Scheme}://{httpRequest.Url.Host}:{httpRequest.Url.Port}");
        HttpMethods method = HttpMethods.UNKNOWN;
        Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
        Dictionary<string, string> queryData = new Dictionary<string, string>();
        Dictionary<string, string> formData = new Dictionary<string, string>();
        List<byte> bodyData = new List<byte>();

        Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

        // build can parse flag and method
        bool canParseMethod = Enum.TryParse(typeof(HttpMethods), httpRequest.HttpMethod, out object? methodObj);
        if (canParseMethod && methodObj != null)
            method = (HttpMethods)methodObj;

        // build headers
        for (int i = 0; i < httpRequest.Headers.Count; i++)
            requestHeaders.Add(
                httpRequest.Headers.GetKey(i) ?? "Unknown",
                httpRequest.Headers.GetValues(i)?[0] ?? "Unknown");

        // build query
        Logger.Trace("current url " + url);
        if (url.Contains("?"))
        {
            var urlParts = url.Split("?");

            url = WebUtility.UrlDecode(urlParts[0]);
            query = urlParts[1] ?? "";

            foreach (var queryPart in query.Split("&"))
            {
                var kvp = queryPart.Split("=");
                queryData.Add(kvp[0], kvp.Length > 1 ? kvp[1]! : "");
            }
        }

        // build content type
        var contentType = HttpContentTypeHelper.ResolveValueFromString(httpRequest.ContentType!);

        // build body
        int ibyte;
        while ((ibyte = httpRequest.InputStream.ReadByte()) != -1)
            bodyData.Add((byte)ibyte);

        if (contentType == HttpContentTypes.UrlencodedForm)
        {
            foreach (var formPart in Encoding.UTF8.GetString(bodyData.ToArray()).Split("&"))
            {
                var kvp = formPart.Split("=");
                if (kvp.Length > 1)
                    formData.Add(kvp[0], kvp[1]);
            }
        }

        Logger.Trace("trimmed url " + url);

        // format url
        FormatUrl(ref url);

        Logger.Trace("formatted url " + url);

        // construct data
        requestData = MgcxmHttpRequest.New(
            method,
            baseAddress,
            url,
            contentType,
            bodyData.ToArray(),
            formData,
            requestHeaders,
            queryData,
            allocatedId);

        responseData = MgcxmHttpResponse.New(
            (HttpStatusCodes)httpResponse.StatusCode,
            "",
            responseHeaders,
            Array.Empty<byte>(),
            allocatedId);
    }

    public static void FormatUrl(ref string url)
    {
        if (url.StartsWith("//"))
            url = url.Replace("//", "/");

        if (url.Length > 1 && url.EndsWith("/"))
            url = url.Remove(url.Length - 1, 1);
    }

    /// <summary>
    /// Starts the HTTP listener to listen for incoming requests.
    /// </summary>
    public void Start()
    {
        _listenToRequests = true;

        _listenThread = new Thread(async () =>
        {
            Logger.Debug($"Changed listen flag on 0x{_allocatedId.Id:x8}");
            await StartListening();
        });
        _listenThread.Start();
    }

    /// <summary>
    /// Starts the HTTP listener internally with a specified listen task.
    /// </summary>
    /// <param name="listentask">The listen task to start the listener.</param>
    internal void StartInternal(AsyncListenTask listentask)
    {
        _listenToRequests = true;

        _listenThread = new Thread(async () =>
        {
            Logger.Debug($"Changed listen flag on 0x{_allocatedId.Id:x8}");
            await listentask(_listener);
        });
        _listenThread.Start();
        GCWrapper.SuppressFinalize(_listenThread);
    }

    /// <summary>
    /// Stops the HTTP listener from listening for incoming requests.
    /// </summary>
    public void Stop()
    {
        _listenToRequests = false;
        _listenThread = null!;
    }

    /// <summary>
    /// Sets the current framework of the server.
    /// </summary>
    /// <remarks>This will block ALL incoming requests to non-frameworked endpoints and redirect them to the framework.</remarks>
    /// <param name="endpointFramework">The framework to use.</param>
    /// <typeparam name="TEndpointUrl">The type of URLs to be used on the framework.</typeparam>
    public void SetFramework<TEndpointUrl>(EndpointFramework<TEndpointUrl> endpointFramework) where TEndpointUrl : EndpointUrl
    {
        _framework = endpointFramework;
        Logger.Trace($"Set current framework pointing 0x{_framework.AllocatedId.Id:x8}");
        GCWrapper.SuppressFinalize(_framework);
    }

    /// <summary>
    /// Adds an endpoint to the framework.
    /// </summary>
    /// <remarks>This method does nothing if you haven't used SetFramework();</remarks>
    /// <param name="url">The URL to point the endpoint to.</param>
    /// <param name="method">The HTTP method used on this endpoint.</param>
    /// <param name="requestedHandler">The handler used to resolve the request and write to the response.</param>
    public void AddFrameworkEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler)
    {
        if (_framework == null)
        {
            Logger.Trace($"Cannot add framework endpoint to server 0x{_allocatedId.Id:x8} because there is no framework existing!");
            return;
        }

        _framework.AddEndpoint(url, method, requestedHandler, this);
    }

    /// <summary>
    /// Gets the allocated ID of the listener.
    /// </summary>
    public MgcxmId AllocatedId => _allocatedId;

    /// <summary>
    /// Gets a value indicating whether the listener is listening for requests.
    /// </summary>
    public bool ListenToRequests => _listenToRequests;

    /// <summary>
    /// Gets the HttpListener instance associated with the listener.
    /// </summary>
    public HttpListener Listener => _listener;

    /// <summary>
    /// Gets a list of all endpoints associated with the listener.
    /// </summary>
    public IReadOnlyList<MgcxmHttpEndpoint> Endpoints => _endpoints;

    /// <summary>
    /// Gets the current framework associated with the listener.
    /// </summary>
    public EndpointFrameworkBase Framework => _framework;

    public X509Certificate2 Certificate { get; private set; }
    public IpAddress Address { get; private set; }

    private MgcxmId _allocatedId;
    private bool _listenToRequests;
    private HttpListener _listener;
    private List<MgcxmHttpEndpoint> _endpoints;
    private EndpointFrameworkBase _framework;
    private Thread _listenThread;
}

/// <summary>
/// Delegate representing an asynchronous listen task for the HTTP listener.
/// </summary>
/// <param name="listener">The HttpListener instance to listen for requests.</param>
public delegate Task AsyncListenTask(HttpListener listener);

#region DEBUG
/// <summary>
/// Represents a debug info handler for MgcxmHttpListener.
/// </summary>
[DebugHandler(typeof(MgcxmHttpListener))]
public sealed class MgcxmHttpListenerDebugInfo : DebugInfoHandler
{
    /// <summary>
    /// Prints the debug information for the MgcxmHttpListener.
    /// </summary>
    /// <param name="target">The target object to print debug information for.</param>
    public override void Print(object target)
    {
        MgcxmHttpListener tgt = target as MgcxmHttpListener;

        Logger.Trace($"Server Id = 0x{tgt.AllocatedId.Id:x8}");
        Logger.Trace($"Listening = {tgt.ListenToRequests.ToString()}");
        Logger.Trace($"Listener = 0x{tgt.Listener.GetHashCode():x8}");
        Logger.Trace($"Endpoint Count = {tgt.Endpoints.Count}");
        Logger.Trace($"Framework = 0x{(tgt.Framework != null ? tgt.Framework.AllocatedId.Id : 0):x8}");
    }
}
#endregion