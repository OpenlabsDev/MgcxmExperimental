// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics.Contracts;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an HTTP endpoint for MgcxmSocketListener.
/// </summary>
public class MgcxmHttpEndpoint
{
    /// <summary>
    /// Initializes a new instance of the MgcxmHttpEndpoint class.
    /// </summary>
    /// <param name="mgcxmId">The MgcxmId of the owner.</param>
    /// <param name="url">The URL of the endpoint.</param>
    /// <param name="dynamicIdentifiers">An array of dynamic identifiers for the endpoint.</param>
    /// <param name="method">The HTTP method used for this endpoint.</param>
    /// <param name="callback">The callback function to be called when the endpoint is requested.</param>
    public MgcxmHttpEndpoint(MgcxmId mgcxmId, string url, MgcxmDynamicIdentifier[] dynamicIdentifiers, HttpMethods method, OnEndpointRequested callback)
    {
        _ownerId = mgcxmId;
        _endpointId = (int)_ownerId.Id + (int)GetHashCode();

        _url = url;
        _dynamicIdentifiers = dynamicIdentifiers;
        _requiredMethod = method;
        _onEndpointRequested = callback;

        MgcxmObjectManager.Register(_endpointId, this);
        _ownerId.Latch(_endpointId, this);
    }

    /// <summary>
    /// Finalizes an instance of the MgcxmHttpEndpoint class.
    /// </summary>
    ~MgcxmHttpEndpoint()
    {
        _ownerId.Unlatch(_endpointId);
        MgcxmObjectManager.Deregister(_endpointId);
    }

    /// <summary>
    /// Checks if the given URL matches the endpoint URL using the MgcxmDynamicIdentifiers and supports dynamic matching.
    /// </summary>
    /// <param name="url">The URL to check for a match.</param>
    /// <param name="endpoint">The MgcxmHttpEndpoint to compare with.</param>
    /// <param name="request">The MgcxmHttpRequest for additional checks.</param>
    /// <returns>True if the URL matches the endpoint; otherwise, false.</returns>
    [Pure]
    public static bool Matches(string url, MgcxmHttpEndpoint endpoint, MgcxmHttpRequest request)
    {
        if (endpoint.RequiredMethod != request.HttpMethod)
            return false;

        // Check if the URL matches
        string[] urlSegments = url.Split('/');
        string[] endpointSegments = endpoint.Url.Split('/');

        if (urlSegments.Length != endpointSegments.Length)
            return false;

        for (int i = 0; i < urlSegments.Length; i++)
        {
            string urlSegment = urlSegments[i];
            string endpointSegment = endpointSegments[i];

            // Check if the segments match exactly or if it's a dynamic identifier
            if (urlSegment != endpointSegment && !IsDynamicIdentifier(endpointSegment))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Helper method to check if a segment is a dynamic identifier (e.g., '{0}', '{1}', etc.).
    /// </summary>
    /// <param name="segment">The segment to check.</param>
    /// <returns>True if the segment is a dynamic identifier; otherwise, false.</returns>
    private static bool IsDynamicIdentifier(string segment) => segment.StartsWith("{") && segment.EndsWith("}");

    /// <summary>
    /// Gets the MgcxmId of the owner.
    /// </summary>
    public MgcxmId OwnerId => _ownerId;

    /// <summary>
    /// Gets the MgcxmId of the endpoint.
    /// </summary>
    public MgcxmId EndpointId => _endpointId;

    /// <summary>
    /// Gets the URL of the endpoint.
    /// </summary>
    public string Url => _url;

    /// <summary>
    /// Gets an array of dynamic identifiers for the endpoint.
    /// </summary>
    public MgcxmDynamicIdentifier[] DynamicIdentifiers => _dynamicIdentifiers;

    /// <summary>
    /// Gets the HTTP method required for this endpoint.
    /// </summary>
    public HttpMethods RequiredMethod => _requiredMethod;

    /// <summary>
    /// Gets the callback function to be called when the endpoint is requested.
    /// </summary>
    public OnEndpointRequested OnEndpointRequested => _onEndpointRequested;

    private MgcxmId _ownerId;
    private MgcxmId _endpointId;
    private string _url;
    private MgcxmDynamicIdentifier[] _dynamicIdentifiers;
    private HttpMethods _requiredMethod;
    private OnEndpointRequested _onEndpointRequested;
}

/// <summary>
/// Represents a dynamic identifier used for matching dynamic URLs.
/// </summary>
public class MgcxmDynamicIdentifier
{
    /// <summary>
    /// Initializes a new instance of the MgcxmDynamicIdentifier class.
    /// </summary>
    /// <param name="index">The index of the dynamic identifier.</param>
    /// <param name="identifier">The identifier string used for matching.</param>
    public MgcxmDynamicIdentifier(int index, string identifier)
    {
        _index = index;
        _identifier = identifier;
    }

    /// <summary>
    /// Gets the index of the dynamic identifier.
    /// </summary>
    public int Index => _index;

    /// <summary>
    /// Gets the identifier string used for matching.
    /// </summary>
    public string Identifier => _identifier;

    private int _index;
    private string _identifier;
}

/// <summary>
/// Delegate representing a callback for handling endpoint requests.
/// </summary>
/// <param name="request">The MgcxmHttpRequest object representing the incoming request.</param>
/// <param name="response">The MgcxmHttpResponse object used to construct the response.</param>
public delegate void OnEndpointRequested(MgcxmHttpRequest request, MgcxmHttpResponse response);
