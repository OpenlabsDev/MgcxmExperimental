// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

/// <summary>
/// Represents an abstract base class for managing a collection of HTTP endpoints associated with a subpart.
/// </summary>
public abstract class EndpointSubpartController
{
    /// <summary>
    /// Notifies the controller that an update occurred in the associated <see cref="EndpointResolver"/>.
    /// </summary>
    /// <param name="resolver">The <see cref="EndpointResolver"/> instance associated with the subpart.</param>
    /// <remarks>
    /// This method is called by the associated <see cref="EndpointResolver"/> when an update occurs in the subpart.
    /// It triggers the <see cref="OnSubpartUpdated"/> event to notify subscribers about the update.
    /// </remarks>
    internal void NotifyUpdate(EndpointResolver resolver)
    {
        if (resolver == null) return;
        OnSubpartUpdated?.Invoke();
    }

    /// <summary>
    /// Adds an <see cref="MgcxmHttpEndpoint"/> to the collection of all endpoints in the subpart.
    /// </summary>
    /// <param name="endpoint">The <see cref="MgcxmHttpEndpoint"/> to add.</param>
    internal void AddEndpoint(MgcxmHttpEndpoint endpoint) => _allEndpoints.Add(endpoint);

    /// <summary>
    /// Gets a read-only list of all the <see cref="MgcxmHttpEndpoint"/> associated with the subpart.
    /// </summary>
    public IReadOnlyList<MgcxmHttpEndpoint> AllEndpoints => _allEndpoints;

    /// <summary>
    /// Event that is triggered when an update occurs in the subpart.
    /// </summary>
    /// <remarks>
    /// Subscribers to this event will be notified when an update occurs in the subpart, allowing them to respond accordingly.
    /// </remarks>
    public event Action OnSubpartUpdated;

    private List<MgcxmHttpEndpoint> _allEndpoints = new List<MgcxmHttpEndpoint>();

    /// <summary>
    /// HTTP request
    /// </summary>
    public MgcxmHttpRequest Request { get; internal set; }

    /// <summary>
    /// HTTP response
    /// </summary>
    public MgcxmHttpResponse Response { get; internal set; }
}