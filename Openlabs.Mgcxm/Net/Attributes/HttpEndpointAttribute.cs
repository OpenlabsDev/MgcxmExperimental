// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to define an HTTP endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class HttpEndpointAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpEndpointAttribute"/> class with the specified HTTP method and URL.
    /// </summary>
    /// <param name="method">The HTTP method associated with the endpoint.</param>
    /// <param name="url">The URL pattern associated with the endpoint.</param>
    public HttpEndpointAttribute(HttpMethods method, string url)
    {
        HttpMethod = method;
        Url = url;
    }

    /// <summary>
    /// Gets the HTTP method associated with the endpoint.
    /// </summary>
    public HttpMethods HttpMethod { get; private set; }

    /// <summary>
    /// Gets the URL pattern associated with the endpoint.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Gets or sets the type of the return value for the endpoint method.
    /// </summary>
    public Type ReturnType { get; set; }
}
