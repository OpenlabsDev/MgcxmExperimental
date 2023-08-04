// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointResolver : IEndpointUrlResolver
{
    public EndpointResolver(MgcxmHttpListener listener, EndpointFrameworkBase endpointFramework)
    {
        HttpListener = listener;
        EndpointFramework = endpointFramework;
    }

    public abstract MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request);
    public abstract void ResolveAll();
    public abstract void AddSubpartController(EndpointSubpartController subpartController);
    
    public EndpointFrameworkBase EndpointFramework { get; private set; }
    public MgcxmHttpListener HttpListener { get; private set; }
    public abstract IReadOnlyList<EndpointSubpartController> EndpointSubpartControllers { get; }
}