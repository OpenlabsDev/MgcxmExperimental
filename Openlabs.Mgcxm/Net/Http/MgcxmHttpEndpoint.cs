// Copr. (c) Nexus 2023. All rights reserved.

using System.Diagnostics.Contracts;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

public class MgcxmHttpEndpoint
{
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

    ~MgcxmHttpEndpoint()
    {
        _ownerId.Unlatch(_endpointId);
        MgcxmObjectManager.Deregister(_endpointId);
    }

    // Method that checks if the url matches the 'endpoint' param
    // using the MgcxmDynamicIdentifiers and can also dynamically match
    // endpoints such as:
    //
    //      - '/api/player/{0}/username'
    //          - '/api/player/1/username'
    //          - '/api/player/5/username'
    //          - '/api/player/10/username'
    //
    // and such
    
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

    // Helper method to check if a segment is a dynamic identifier (e.g., '{0}', '{1}', etc.)
    private static bool IsDynamicIdentifier(string segment) => segment.StartsWith("{") && segment.EndsWith("}");

    public MgcxmId OwnerId => _ownerId;
    public MgcxmId EndpointId => _endpointId;
    public string Url => _url;
    public MgcxmDynamicIdentifier[] DynamicIdentifiers => _dynamicIdentifiers;
    public HttpMethods RequiredMethod => _requiredMethod;
    public OnEndpointRequested OnEndpointRequested => _onEndpointRequested;

    private MgcxmId _ownerId;
    private MgcxmId _endpointId;
    private string _url;
    private MgcxmDynamicIdentifier[] _dynamicIdentifiers;
    private HttpMethods _requiredMethod;
    private OnEndpointRequested _onEndpointRequested;
}

public class MgcxmDynamicIdentifier
{
    public MgcxmDynamicIdentifier(int index, string identifier)
    {
        _index = index;
        _identifier = identifier;
    }
    
    public int Index => _index;
    public string Identifier => _identifier;

    private int _index;
    private string _identifier;
}

public delegate void OnEndpointRequested(MgcxmHttpRequest request, MgcxmHttpResponse response);