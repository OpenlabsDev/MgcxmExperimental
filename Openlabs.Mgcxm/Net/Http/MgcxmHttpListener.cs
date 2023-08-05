// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using System.Reflection;
using System.Text;
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
public class MgcxmHttpListener
{
    /// <summary>
    /// Initializes a new instance of the MgcxmHttpListener class.
    /// </summary>
    /// <param name="addressToHostOn">The IP address to host the listener on.</param>
    public MgcxmHttpListener(IpAddress addressToHostOn)
    {
        // Initialize the HttpListener with the provided IP address and port.
        _listener = new HttpListener();
        _listener.Prefixes.Add(
            $"http://{AddressUtilities.FormatIpAddress(addressToHostOn.Origin)}:{addressToHostOn.Port}/");

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
                response.Status(HttpStatusCodes.OK).Content(HttpContentTypes.BinaryData, Array.Empty<byte>());
            }));
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
