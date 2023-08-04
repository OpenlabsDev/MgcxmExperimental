// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpUseAuthorizationAttribute : Attribute
{
    public HttpUseAuthorizationAttribute(AuthorizationSchemes scheme)
    {
        Scheme = scheme;
    }
    
    public AuthorizationSchemes Scheme { get; private set; }
}