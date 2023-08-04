// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointStructure<TEndpointUrl> 
    : IEndpointUrlResolver 
    where TEndpointUrl : EndpointUrl
{
    public abstract MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request);

    public abstract void AddEndpoint(EndpointUrl url, HttpMethods method, OnEndpointRequested requestedHandler, MgcxmHttpListener listener);

    public abstract string ApiPath { get; }
    public abstract IReadOnlyDictionary<TEndpointUrl, MgcxmHttpEndpoint> Endpoints { get; }
}