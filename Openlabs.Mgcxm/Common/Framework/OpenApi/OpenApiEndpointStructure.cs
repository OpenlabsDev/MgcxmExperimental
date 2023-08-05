// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Common.Framework;

public class OpenApiEndpointStructure : EndpointStructure<OpenApiUrl>
{
    public OpenApiEndpointStructure()
    {
        _allocatedId = Random.Range(900000, 10000000);
        MgcxmObjectManager.Register(_allocatedId, this);
    }
    ~OpenApiEndpointStructure() => MgcxmObjectManager.Deregister(_allocatedId);
    
    public override MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request)
    {
        if (!url.StartsWith(ApiPath))
            throw new OpenApiEndpointSearchException($"Cannot find '{url}'. The url does not start with '{ApiPath}' and is not a valid OpenAPI endpoint.");

        foreach (var vEndpoint in Endpoints)
        {
            // resolve the endpoint
            Logger.Trace($"Endpoint = {vEndpoint.Key.ToString()}, Url = {url}");
            bool isMatched = MgcxmHttpEndpoint.Matches(vEndpoint.Key.ToString(), vEndpoint.Value, request);
            if (isMatched) return vEndpoint.Value;
        }
        return null!;
    }

    public override void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener)
    {
        var openApiUrl = (OpenApiUrl)url;
        _allEndpoints.Add(openApiUrl, new MgcxmHttpEndpoint(listener.AllocatedId, openApiUrl.ToString(), new MgcxmDynamicIdentifier[0], method, requestedHandler));
    }

    public MgcxmId AllocatedId => _allocatedId;
    public override string ApiPath => "/api/";
    public override IReadOnlyDictionary<OpenApiUrl, MgcxmHttpEndpoint> Endpoints => _allEndpoints;

    private MgcxmId _allocatedId;
    private Dictionary<OpenApiUrl, MgcxmHttpEndpoint> _allEndpoints = new();
}

public class OpenApiEndpointSearchException : Exception
{
    public OpenApiEndpointSearchException() { }
    public OpenApiEndpointSearchException(string message) : base(message) { }
    public OpenApiEndpointSearchException(string message, Exception inner) : base(message, inner) { }
}