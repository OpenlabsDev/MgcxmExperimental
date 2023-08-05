// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;

using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Common.Framework;

public class OpenApiFramework : EndpointFramework<OpenApiUrl>
{
    public OpenApiFramework(MgcxmHttpListener listener)
    {
        _allocatedId = Random.Range(900000, 10000000);
        _endpointResolver = new OpenApiEndpointResolver(listener, this);
        
        MgcxmObjectManager.Register(_allocatedId, this);
    }

    ~OpenApiFramework() => Trash();
    
    public override async Task ResolveRequest(
        HttpListenerRequest listenerRequest, 
        HttpListenerResponse listenerResponse, 
        MgcxmHttpRequest mgcxmRequest, 
        MgcxmHttpResponse mgcxmResponse)
    {
        MgcxmHttpEndpoint endpoint = EndpointResolver.ResolveUrl(mgcxmRequest.Uri, mgcxmRequest);
        endpoint.OnEndpointRequested(mgcxmRequest, mgcxmResponse);

        await mgcxmResponse.Transfer(listenerResponse);
    }

    public override void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener)
        => EndpointStructure.AddEndpoint(url, method, requestedHandler, listener);

    public override void Trash() => MgcxmObjectManager.Deregister(_allocatedId);

    public override MgcxmId AllocatedId => _allocatedId;
    public override IReadOnlyList<MgcxmHttpEndpoint> Endpoints => this.EndpointStructure.Endpoints.Values.ToList();
    public override EndpointStructure<OpenApiUrl> EndpointStructure { get; } = new OpenApiEndpointStructure();
    public override EndpointResolver EndpointResolver => _endpointResolver;

    private MgcxmId _allocatedId;
    private OpenApiEndpointResolver _endpointResolver;
}