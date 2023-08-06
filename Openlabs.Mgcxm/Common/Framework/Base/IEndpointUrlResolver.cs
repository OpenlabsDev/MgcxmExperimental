// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// Represents a contract for resolving an HTTP endpoint URL to a specific <see cref="MgcxmHttpEndpoint"/> instance.
/// </summary>
public interface IEndpointUrlResolver
{
    /// <summary>
    /// Resolves the given URL and HTTP request to an <see cref="MgcxmHttpEndpoint"/> instance.
    /// </summary>
    /// <param name="url">The URL to resolve.</param>
    /// <param name="request">The HTTP request associated with the URL.</param>
    /// <returns>An instance of <see cref="MgcxmHttpEndpoint"/> representing the resolved endpoint.</returns>
    /// <remarks>
    /// This method is responsible for mapping the provided URL and HTTP request to a specific
    /// <see cref="MgcxmHttpEndpoint"/> instance that corresponds to the desired endpoint.
    /// </remarks>
    MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request);
}
