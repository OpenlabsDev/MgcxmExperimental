// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class HttpEndpointAttribute : Attribute
{
    public HttpEndpointAttribute(HttpMethods method, string url)
    {
        HttpMethod = method;
        Url = url;
    }
    
    public HttpMethods HttpMethod { get; private set; }
    public string Url { get; private set; }
    
    public Type ReturnType { get; set; }
}