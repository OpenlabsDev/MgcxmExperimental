// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointFrameworkBase : IMgcxmSystemObject
{
    public abstract Task ResolveRequest(
        HttpListenerRequest listenerRequest, 
        HttpListenerResponse listenerResponse, 
        MgcxmHttpRequest mgcxmRequest, 
        MgcxmHttpResponse mgcxmResponse);

    public abstract void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener);

    public abstract void Trash();
    
    public abstract IReadOnlyList<MgcxmHttpEndpoint> Endpoints { get; }
    public abstract EndpointResolver EndpointResolver { get; }
    public abstract MgcxmId AllocatedId { get; }
}