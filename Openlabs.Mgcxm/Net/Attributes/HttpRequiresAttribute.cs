// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to specify additional requirements for an HTTP endpoint method.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public sealed class HttpRequiresAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequiresAttribute"/> class with the specified HTTP requirement flag and optional data.
    /// </summary>
    /// <param name="flag">The HTTP requirement flag associated with the attribute.</param>
    /// <param name="data">Optional data associated with the requirement, if applicable.</param>
    public HttpRequiresAttribute(HttpRequireFlag flag, object data = null)
    {
        Flag = flag;
        Data = data;
    }

    /// <summary>
    /// Gets the HTTP requirement flag associated with the attribute.
    /// </summary>
    public HttpRequireFlag Flag { get; private set; }

    /// <summary>
    /// Gets optional data associated with the requirement, if applicable.
    /// </summary>
    public object Data { get; private set; }
}

/// <summary>
/// An enumeration of flags that represent additional requirements for an HTTP endpoint method.
/// </summary>
public enum HttpRequireFlag
{
    /// <summary>
    /// Indicates that the HTTP endpoint requires form data in the request body.
    /// </summary>
    FormBody,

    /// <summary>
    /// Indicates that the HTTP endpoint requires binary data in the request body.
    /// </summary>
    BinaryData,

    /// <summary>
    /// Indicates that the HTTP endpoint requires specific headers in the request.
    /// </summary>
    Header,

    /// <summary>
    /// Indicates that the HTTP endpoint requires specific query parameters in the request.
    /// </summary>
    Query
}