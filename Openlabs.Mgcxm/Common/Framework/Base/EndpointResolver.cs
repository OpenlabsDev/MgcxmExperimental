// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// Represents an abstract base class for resolving and managing HTTP endpoints associated with a specific <see cref="MgcxmHttpListener"/>.
/// </summary>
public abstract class EndpointResolver : IEndpointUrlResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointResolver"/> class with the specified listener and endpoint framework.
    /// </summary>
    /// <param name="listener">The <see cref="MgcxmHttpListener"/> instance used to handle HTTP requests.</param>
    /// <param name="endpointFramework">The <see cref="EndpointFrameworkBase"/> associated with this resolver.</param>
    public EndpointResolver(MgcxmHttpListener listener, EndpointFrameworkBase endpointFramework)
    {
        HttpListener = listener;
        EndpointFramework = endpointFramework;
    }

    /// <summary>
    /// Resolves the provided URL and request to an <see cref="MgcxmHttpEndpoint"/> if it matches any of the registered endpoints.
    /// </summary>
    /// <param name="url">The URL to resolve.</param>
    /// <param name="request">The HTTP request associated with the URL.</param>
    /// <returns>
    /// The <see cref="MgcxmHttpEndpoint"/> instance that matches the provided URL and request, or null if no match is found.
    /// </returns>
    public abstract MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request);

    /// <summary>
    /// Resolves all registered HTTP endpoints within the resolver.
    /// </summary>
    public abstract void ResolveAll();

    /// <summary>
    /// Adds an <see cref="EndpointSubpartController"/> to the list of subpart controllers managed by this resolver.
    /// </summary>
    /// <param name="subpartController">The <see cref="EndpointSubpartController"/> to add.</param>
    public abstract void AddSubpartController(EndpointSubpartController subpartController);

    /// <summary>
    /// Gets the <see cref="EndpointFrameworkBase"/> associated with this resolver.
    /// </summary>
    public EndpointFrameworkBase EndpointFramework { get; private set; }

    /// <summary>
    /// Gets the <see cref="MgcxmHttpListener"/> instance used to handle HTTP requests.
    /// </summary>
    public MgcxmHttpListener HttpListener { get; private set; }

    /// <summary>
    /// Gets a read-only list of <see cref="EndpointSubpartController"/> instances managed by this resolver.
    /// </summary>
    public abstract IReadOnlyList<EndpointSubpartController> EndpointSubpartControllers { get; }
}
