// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents a route for WebSocket communication in MgcxmSocketListener.
/// </summary>
public class MgcxmSocketRoute
{
    /// <summary>
    /// Initializes a new instance of the MgcxmSocketRoute class with the specified parameters.
    /// </summary>
    /// <param name="mgcxmId">The MgcxmId of the owner of the route.</param>
    /// <param name="route">The route URL associated with the WebSocket route.</param>
    /// <param name="requireAuthorization">Indicates if the route requires authorization.</param>
    /// <param name="onSocketUpgraded">The delegate to be invoked when a WebSocket is upgraded.</param>
    /// <param name="methodInfo">The MethodInfo representing the method to be invoked for the route.</param>
    public MgcxmSocketRoute(MgcxmId mgcxmId, string route, bool requireAuthorization, OnSocketUpgraded onSocketUpgraded, MethodInfo methodInfo)
    {
        _ownerId = mgcxmId;
        _routeId = (int)_ownerId.Id + (int)GetHashCode();

        _route = route;
        _requireAuthorization = requireAuthorization;

        _onSocketUpgraded = onSocketUpgraded;

        MethodInfo = methodInfo;

        MgcxmObjectManager.Register(_routeId, this);
        _ownerId.Latch(_routeId, this);
    }

    /// <summary>
    /// Finalizer (destructor) for the MgcxmSocketRoute class.
    /// </summary>
    ~MgcxmSocketRoute()
    {
        _ownerId.Unlatch(_routeId);
        MgcxmObjectManager.Deregister(_routeId);
    }

    /// <summary>
    /// Gets the MgcxmId of the owner of the route.
    /// </summary>
    public MgcxmId OwnerId => _ownerId;

    /// <summary>
    /// Gets the unique identifier (route ID) of the route.
    /// </summary>
    public MgcxmId RouteId => _routeId;

    /// <summary>
    /// Gets the route URL associated with the WebSocket route.
    /// </summary>
    public string Route => _route;

    /// <summary>
    /// Gets a value indicating whether the route requires authorization.
    /// </summary>
    public bool RequireAuthorization => _requireAuthorization;

    /// <summary>
    /// Gets the delegate to be invoked when a WebSocket is upgraded.
    /// </summary>
    public OnSocketUpgraded OnSocketUpgraded => _onSocketUpgraded;

    /// <summary>
    /// Gets the MethodInfo representing the method to be invoked for the route.
    /// </summary>
    public MethodInfo MethodInfo { get; }

    private MgcxmId _ownerId;
    private MgcxmId _routeId;
    private string _route;
    private bool _requireAuthorization;
    private OnSocketUpgraded _onSocketUpgraded;
}

/// <summary>
/// Represents a delegate for the callback function used when a WebSocket is upgraded.
/// </summary>
/// <param name="socket">The MgcxmSocket representing the upgraded WebSocket.</param>
public delegate void OnSocketUpgraded(MgcxmSocket socket);