// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

public class MgcxmSocketRoute
{
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

    ~MgcxmSocketRoute()
    {
        _ownerId.Unlatch(_routeId);
        MgcxmObjectManager.Deregister(_routeId);
    }
    
    public MgcxmId OwnerId => _ownerId;
    public MgcxmId RouteId => _routeId;
    public string Route => _route;
    public bool RequireAuthorization => _requireAuthorization;
    public OnSocketUpgraded OnSocketUpgraded => _onSocketUpgraded;
    public MethodInfo MethodInfo { get; }

    private MgcxmId _ownerId;
    private MgcxmId _routeId;
    private string _route;
    private bool _requireAuthorization;
    private OnSocketUpgraded _onSocketUpgraded;
}

public delegate void OnSocketUpgraded(MgcxmSocket socket);