// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// Represents an abstract base class for an endpoint framework used to handle HTTP requests and manage endpoints.
/// </summary>
public abstract class EndpointFrameworkBase : IMgcxmSystemObject
{
    /// <summary>
    /// Resolves the incoming HTTP request and generates an appropriate response using the provided request and response objects.
    /// </summary>
    /// <param name="listenerRequest">The <see cref="HttpListenerRequest"/> object representing the incoming HTTP request.</param>
    /// <param name="listenerResponse">The <see cref="HttpListenerResponse"/> object used to generate the HTTP response.</param>
    /// <param name="mgcxmRequest">The <see cref="MgcxmHttpRequest"/> object associated with the HTTP request.</param>
    /// <param name="mgcxmResponse">The <see cref="MgcxmHttpResponse"/> object used to construct the HTTP response.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation of handling the HTTP request.</returns>
    public abstract Task ResolveRequest(
        HttpListenerRequest listenerRequest,
        HttpListenerResponse listenerResponse,
        MgcxmHttpRequest mgcxmRequest,
        MgcxmHttpResponse mgcxmResponse);

    /// <summary>
    /// Adds an HTTP endpoint to the framework with the specified URL, HTTP method, and request handler.
    /// </summary>
    /// <param name="url">The <see cref="EndpointUrl"/> representing the URL of the endpoint.</param>
    /// <param name="method">The <see cref="HttpMethods"/> representing the HTTP method of the endpoint.</param>
    /// <param name="requestedHandler">The <see cref="OnEndpointRequested"/> delegate representing the request handler for the endpoint.</param>
    /// <param name="listener">The <see cref="MgcxmHttpListener"/> to associate with the endpoint.</param>
    public abstract void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener);

    /// <summary>
    /// Performs any necessary cleanup and shutdown tasks for the endpoint framework.
    /// </summary>
    public abstract void Trash();

    /// <summary>
    /// Gets a read-only list of all registered <see cref="MgcxmHttpEndpoint"/> instances in the framework.
    /// </summary>
    public abstract IReadOnlyList<MgcxmHttpEndpoint> Endpoints { get; }

    /// <summary>
    /// Gets the <see cref="EndpointResolver"/> associated with the endpoint framework.
    /// </summary>
    public abstract EndpointResolver EndpointResolver { get; }

    /// <summary>
    /// Gets the unique identifier assigned to the framework.
    /// </summary>
    public abstract MgcxmId AllocatedId { get; }
}
