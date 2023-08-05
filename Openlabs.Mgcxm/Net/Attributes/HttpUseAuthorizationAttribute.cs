// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to specify the authorization scheme required for an HTTP endpoint method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpUseAuthorizationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpUseAuthorizationAttribute"/> class with the specified authorization scheme.
    /// </summary>
    /// <param name="scheme">The authorization scheme required for the HTTP endpoint.</param>
    public HttpUseAuthorizationAttribute(AuthorizationSchemes scheme)
    {
        Scheme = scheme;
    }

    /// <summary>
    /// Gets the authorization scheme required for the HTTP endpoint.
    /// </summary>
    public AuthorizationSchemes Scheme { get; private set; }
}
