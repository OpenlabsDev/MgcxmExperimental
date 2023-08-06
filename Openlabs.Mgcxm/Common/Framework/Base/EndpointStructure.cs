// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// Represents an abstract base class for managing a collection of HTTP endpoints associated with a specific <typeparamref name="TEndpointUrl"/>.
/// </summary>
/// <typeparam name="TEndpointUrl">The type of the specific <see cref="EndpointUrl"/> used in this structure.</typeparam>
public abstract class EndpointStructure<TEndpointUrl> : IEndpointUrlResolver where TEndpointUrl : EndpointUrl
{
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
    /// Adds an endpoint to the collection of registered endpoints associated with a specific <see cref="EndpointUrl"/>.
    /// </summary>
    /// <param name="url">The <see cref="EndpointUrl"/> instance to associate with the endpoint.</param>
    /// <param name="method">The HTTP method supported by the endpoint.</param>
    /// <param name="requestedHandler">The callback method to be executed when the endpoint is requested.</param>
    /// <param name="listener">The <see cref="MgcxmHttpListener"/> instance to handle the HTTP requests.</param>
    public abstract void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener);

    /// <summary>
    /// Gets the API path associated with this endpoint structure.
    /// </summary>
    /// <remarks>
    /// The API path is a common prefix that is added to all endpoints within this structure.
    /// </remarks>
    public abstract string ApiPath { get; }

    /// <summary>
    /// Gets a read-only dictionary containing all registered endpoints associated with the specific <typeparamref name="TEndpointUrl"/>.
    /// </summary>
    public abstract IReadOnlyDictionary<TEndpointUrl, MgcxmHttpEndpoint> Endpoints { get; }
}
