// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class HttpDynamicUseAttribute : Attribute
{
    public HttpDynamicUseAttribute(params string[] dynamicIdentifiers)
    {
        DynamicIdentifiers = dynamicIdentifiers;
    }
    
    public string[] DynamicIdentifiers { get; private set; }
}