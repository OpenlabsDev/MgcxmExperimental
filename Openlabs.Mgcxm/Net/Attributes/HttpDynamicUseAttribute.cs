// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to specify dynamic identifiers for a method handling an HTTP endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class HttpDynamicUseAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpDynamicUseAttribute"/> class with the specified dynamic identifiers.
    /// </summary>
    /// <param name="dynamicIdentifiers">The dynamic identifiers associated with the method.</param>
    public HttpDynamicUseAttribute(params string[] dynamicIdentifiers)
    {
        DynamicIdentifiers = dynamicIdentifiers;
    }

    /// <summary>
    /// Gets the array of dynamic identifiers associated with the method.
    /// </summary>
    public string[] DynamicIdentifiers { get; private set; }
}