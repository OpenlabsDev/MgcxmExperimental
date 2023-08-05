// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Common.Framework;

public class OpenApiEndpointResolver : EndpointResolver
{
    public OpenApiEndpointResolver(MgcxmHttpListener listener, OpenApiFramework framework) : base(listener, framework)
    {
        _allocatedId = Random.Range(900000, 10000000);
        MgcxmObjectManager.Register(_allocatedId, this);
    }
    ~OpenApiEndpointResolver() => MgcxmObjectManager.Deregister(_allocatedId);

    public override MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request)
    {
        if (OpenApiFramework == null)
        {
            Logger.Error("Cannot resolve url because the OpenApiFramework is null.");
            return null!;
        }
        
        if (!url.StartsWith(OpenApiEndpointStructure!.ApiPath))
            throw new OpenApiEndpointSearchException($"Cannot find '{url}'. The url does not start with '{OpenApiEndpointStructure.ApiPath}' and is not a valid OpenAPI endpoint.");

        foreach (var vEndpoint in OpenApiEndpointStructure.Endpoints)
        {
            Logger.Trace("Resolving " + vEndpoint.Key.ToString());
            
            // resolve the endpoint
            bool isMatched = MgcxmHttpEndpoint.Matches(url, vEndpoint.Value, request);
            if (isMatched)
            {
                Logger.Trace("Url matching succeeded.");
                Logger.Trace($"Endpoint = {vEndpoint.Key.ToString()}, Url = {url}");
                return vEndpoint.Value;
            }
        }
        
        Logger.Error("Cannot resolve url because no endpoints were found.");
        return null!;
    }

    public override void ResolveAll()
    {
        foreach (var subpartController in _endpointSubpartControllers)
        {
            var type = subpartController.GetType();
            Logger.Trace($"Resolving subpart '{type.FullName}'");
            
            AttributeResolver.ResolveAllMethod<HttpEndpointAttribute>(
                type,
                typeof(HttpEndpointAttribute),
                (method, attr) =>
                {
                    if (subpartController.AllEndpoints.Any(x => x.Url == attr.Url))
                    {
                        Logger.Error($"Cannot resolve subpart - there is already a endpoint with the url '{attr.Url}'");
                        return; // cannot add endpoints with same url
                    }
                    

                    OpenApiEndpointStructure!.AddEndpoint(EndpointUrl.FromString<OpenApiUrl>(attr.Url), attr.HttpMethod,
                        (req, res) =>
                        {
                            var parameters = method.GetParameters();
                            bool isInvalidSig = parameters.Count() != 2 ||
                                                (parameters[0].ParameterType != typeof(MgcxmHttpRequest) &&
                                                 parameters[1].ParameterType != typeof(MgcxmHttpResponse));

                            if (isInvalidSig) return; // cannot add endpoints with invalid signatures
                            method.Invoke(subpartController, new object[] { req, res });
                        }, HttpListener);
                });
        }
    }

    public override void AddSubpartController(EndpointSubpartController subpartController)
    {
        _endpointSubpartControllers.Add(subpartController);
        
        subpartController.NotifyUpdate(this);
        OnSubpartUpdated!.InvokeSafe(subpartController);
    }

    public MgcxmId AllocatedId => _allocatedId;
    public event Action<EndpointSubpartController>? OnSubpartUpdated;
    public override IReadOnlyList<EndpointSubpartController> EndpointSubpartControllers => _endpointSubpartControllers;
    public OpenApiFramework? OpenApiFramework => EndpointFramework as OpenApiFramework;
    public OpenApiEndpointStructure? OpenApiEndpointStructure => OpenApiFramework!.EndpointStructure as OpenApiEndpointStructure;

    private readonly MgcxmId _allocatedId;
    private readonly List<EndpointSubpartController> _endpointSubpartControllers = new();
}