// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using System.Reflection;
using System.Text;
using Openlabs.Mgcxm.Common.Framework;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net.Extensions;

namespace Openlabs.Mgcxm.Net;

public static class AddressUtilities
{
    public static string FormatIpAddress(string addr)
    {
        if (addr == "localhost") return "+";
        return addr;
    }
}

public class MgcxmHttpListener
{
    public MgcxmHttpListener(IpAddress addressToHostOn)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(
            $"http://{AddressUtilities.FormatIpAddress(addressToHostOn.Origin)}:{addressToHostOn.Port}/");

        _allocatedId = GetHashCode();

        _endpoints = new List<MgcxmHttpEndpoint>();
        Logger.Debug(string.Format("Created ({0}, 0x{1:x8}) for '{2}:{3}'", GetType().Name, _allocatedId.Id,
            addressToHostOn.Origin, addressToHostOn.Port));
        InitializeCore(); // initialize /favicon.ico and other systems

        MgcxmObjectManager.Register(_allocatedId, this); // register object
        AttributeResolver.ResolveAllMethod<HttpEndpointAttribute>(
            GetType(),
            typeof(HttpEndpointAttribute),
            (method, attr) =>
            {
                if (_endpoints.Any(x => x.Url == attr.Url))
                    return; // cannot add endpoints with same url

                var dynamicUseAttribute = method.GetCustomAttribute<HttpDynamicUseAttribute>();
                
                string[] urlParts = attr.Url.Split("/");
                List<MgcxmDynamicIdentifier> currentIdentifiers = new List<MgcxmDynamicIdentifier>();
                for (int i = 0; i < urlParts.Length; i++)
                {
                    if (dynamicUseAttribute != null && dynamicUseAttribute!.DynamicIdentifiers.Contains(urlParts[i]))
                    {
                        currentIdentifiers.Add(
                            new MgcxmDynamicIdentifier(
                                i, 
                                dynamicUseAttribute!.DynamicIdentifiers.First(x => x == urlParts[i])));
                    }
                }
                
                attr.ReturnType = method.ReturnType;
                
                //
                // string[] dynamicIdentifiers 
                //      = dynamicUseAttribute != null
                //          ? dynamicUseAttribute.DynamicIdentifiers 
                //          : Array.Empty<string>();
                //
                
                _endpoints.Add(new MgcxmHttpEndpoint(
                    _allocatedId, attr.Url, currentIdentifiers.ToArray(), attr.HttpMethod,
                    (req, res) =>
                    {
                        var parameters = method.GetParameters();
                        bool isInvalidSig = parameters.Count() != 2 ||
                                            (parameters[0].ParameterType != typeof(MgcxmHttpRequest) &&
                                             parameters[1].ParameterType != typeof(MgcxmHttpResponse));

                        if (isInvalidSig) return; // cannot add endpoints with invalid signatures
                        method.Invoke(this, new object[] { req, res });
                    }));
                Logger.Trace(string.Format("Added '0x{0:x8}' to '0x{1:x8}'", attr.Url.GetHashCode(),
                    _allocatedId.Id));
            });

        // this.Start();
    }

    ~MgcxmHttpListener() => MgcxmObjectManager.Deregister(_allocatedId);

    private void InitializeCore()
    {
        const long FAVICON_ID = 2441798;

        Logger.Debug($"Creating identifier 0x{FAVICON_ID:x8} pointing 0x{_allocatedId.Id:x8}");

        // create /favicon.ico page uri pointing 0x00254246
        _endpoints.Add(new MgcxmHttpEndpoint(FAVICON_ID, "/favicon.ico", Array.Empty<MgcxmDynamicIdentifier>(), HttpMethods.GET,
            (request, response) =>
            {
                response.Status(HttpStatusCodes.OK).Content(HttpContentTypes.BinaryData, Array.Empty<byte>());
            }));
    }

    private async Task StartListening()
    {
        this.PrintDebugInfo();

        _listener.Start();
        _listenToRequests = true;

        while (_listenToRequests)
        {
            var httpContext = await _listener.GetContextAsync();

            var httpRequest = httpContext.Request;
            var httpResponse = httpContext.Response;
            
            // build needed data
            string query = "";
            string url = httpRequest.Url!.PathAndQuery;

            Uri baseAddress = new Uri($"{httpRequest.Url.Scheme}://{httpRequest.Url.Host}:{httpRequest.Url.Port}");
            HttpMethods method = HttpMethods.UNKNOWN;
            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
            Dictionary<string, string> queryData = new Dictionary<string, string>();
            Dictionary<string, string> formData = new Dictionary<string, string>();
            List<byte> bodyData = new List<byte>();

            Dictionary<MgcxmString, MgcxmString> responseHeaders = new Dictionary<MgcxmString, MgcxmString>();

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

            // build content type
            var contentType = HttpContentTypeHelper.ResolveValue(httpRequest.ContentType!);

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

            // format url
            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1, 1);

            // construct data
            var requestData = MgcxmHttpRequest.New(
                method,
                baseAddress,
                url,
                contentType,
                bodyData.ToArray(),
                formData,
                requestHeaders,
                queryData,
                _allocatedId);

            var responseData = MgcxmHttpResponse.New(
                (HttpStatusCodes)httpResponse.StatusCode,
                "",
                responseHeaders,
                Array.Empty<byte>(),
                _allocatedId);

            //Logger.Info($"=========== Ws Request ===========");
            //Logger.Info($"==================================");

            // log request
            Logger.Info($"||==----------- Http Request -----------==||");
            Logger.Info($"Server Id = 0x{_allocatedId.Id:x8}");
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
            
            httpResponse.KeepAlive = true;
            responseData.Header("Connection", "keep-alive");
            
            // write to response
            if (_framework == null)
            {
                var endpoint = _endpoints.Find(x =>
                    x.Url == requestData.Uri && x.RequiredMethod == Enum.Parse<HttpMethods>(httpRequest.HttpMethod));
                if (endpoint != null)
                {
                    endpoint.OnEndpointRequested(requestData, responseData);
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
                                // Logger.Trace($"0x{vEndpoint.EndpointId.Id:x8} meets dynamic criterion");
                                vEndpoint.OnEndpointRequested(requestData, responseData);
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
            else
            {
                try
                {
                    await _framework.ResolveRequest(httpRequest, httpResponse, requestData, responseData);
                }
                catch (Exception exception)
                {
                    Logger.Exception("Cannot resolve Framework endpoint", exception);
                }
            }
        }

        _listener.Stop();
    }

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

    internal void StartInternal(AsyncListenTask listentask)
    {
        _listenToRequests = true;

        _listenThread = new Thread(async () =>
        {
            Logger.Debug($"Changed listen flag on 0x{_allocatedId.Id:x8}");
            await listentask(_listener);
        });
        _listenThread.Start();
    }

    public void Stop()
    {
        _listenToRequests = false;
        _listenThread = null!;
    }

    /// <summary>
    /// Sets the current framework of the server.
    /// <remarks>This will block ALL incoming requests to non-frameworked endpoints and redirect them to the framework.</remarks>
    /// </summary>
    /// <param name="endpointFramework">The framework to use</param>
    /// <typeparam name="TEndpointUrl">The type of URLs to be used on the framework.</typeparam>
    public void SetFramework<TEndpointUrl>(EndpointFramework<TEndpointUrl> endpointFramework) where TEndpointUrl : EndpointUrl
    {
        _framework = endpointFramework;
        Logger.Trace($"Set current framework pointing 0x{_framework.AllocatedId.Id:x8}");
    }

    /// <summary>
    /// Adds an endpoint to the framework.
    /// <remarks>This method does nothing if you haven't used SetFramework();</remarks>
    /// </summary>
    /// <param name="url">The url to point the endpoint to.</param>
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

    public MgcxmId AllocatedId => _allocatedId;
    public bool ListenToRequests => _listenToRequests;
    public HttpListener Listener => _listener;
    public IReadOnlyList<MgcxmHttpEndpoint> Endpoints => _endpoints;
    public EndpointFrameworkBase Framework => _framework;

    private MgcxmId _allocatedId;
    private bool _listenToRequests;
    private HttpListener _listener;
    private List<MgcxmHttpEndpoint> _endpoints;
    private EndpointFrameworkBase _framework;

    private Thread _listenThread;
}
    
public delegate Task AsyncListenTask(HttpListener listener);

[DebugHandler(typeof(MgcxmHttpListener))]
public sealed class MgcxmHttpListenerDebugInfo : DebugInfoHandler
{
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