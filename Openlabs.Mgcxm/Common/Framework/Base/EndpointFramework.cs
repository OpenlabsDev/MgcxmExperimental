// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointFramework<TEndpointUrl> : EndpointFrameworkBase where TEndpointUrl : EndpointUrl
{
    public abstract EndpointStructure<TEndpointUrl> EndpointStructure { get; }
    
}