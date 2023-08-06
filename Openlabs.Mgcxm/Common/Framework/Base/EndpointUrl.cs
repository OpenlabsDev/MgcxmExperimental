// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// An abstract base class representing an endpoint URL.
/// </summary>
public abstract class EndpointUrl
{
    /// <summary>
    /// Returns a string representation of the endpoint URL.
    /// </summary>
    /// <returns>A string representation of the endpoint URL.</returns>
    public new abstract string ToString();

    /// <summary>
    /// Populates the endpoint URL object from the given URL string.
    /// </summary>
    /// <param name="url">The URL string to populate the object from.</param>
    public abstract void FromString(string url);

    /// <summary>
    /// Creates an instance of a specific <typeparamref name="TEndpointUrl"/> and populates it from the given URL string.
    /// </summary>
    /// <typeparam name="TEndpointUrl">The type of the endpoint URL to create.</typeparam>
    /// <param name="url">The URL string to populate the endpoint URL object from.</param>
    /// <returns>An instance of the specified <typeparamref name="TEndpointUrl"/> populated with the given URL string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the given URL string is null.</exception>
    public static TEndpointUrl FromString<TEndpointUrl>(string url)
        where TEndpointUrl : EndpointUrl
    {
        if (url == null)
            throw new ArgumentNullException(nameof(url));

        var epUrl = Activator.CreateInstance<TEndpointUrl>();
        epUrl.FromString(url);
        return epUrl;
    }
}