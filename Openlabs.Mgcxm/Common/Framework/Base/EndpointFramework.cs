// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// A generic abstract class representing an endpoint framework that handles HTTP endpoints.
/// </summary>
/// <typeparam name="TEndpointUrl">The type of endpoint URL used in the framework.</typeparam>
public abstract class EndpointFramework<TEndpointUrl> : EndpointFrameworkBase where TEndpointUrl : EndpointUrl
{
    /// <summary>
    /// Gets the endpoint structure associated with this endpoint framework.
    /// </summary>
    public abstract EndpointStructure<TEndpointUrl> EndpointStructure { get; }
}
