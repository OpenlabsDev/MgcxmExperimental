// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

public class SocketRouteAttribute : Attribute
{
    public SocketRouteAttribute(string route)
    {
        Route = route;
    }
    
    public string Route { get; private set; }
}