// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;

namespace Openlabs.Mgcxm.Net;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class HttpRequiresAttribute : Attribute
{
    public HttpRequiresAttribute(HttpRequireFlag flag, object data = null)
    {
        this.Flag = flag;
        this.Data = data;
    }
    
    public HttpRequireFlag Flag { get; private set; }
    public object Data { get; private set; }
}

public enum HttpRequireFlag
{
    FormBody,
    BinaryData,
    Header,
    Query
}