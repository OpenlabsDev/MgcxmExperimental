// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to specify the route for a socket connection.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class SocketRouteAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SocketRouteAttribute"/> class with the specified route.
    /// </summary>
    /// <param name="route">The route for the socket connection.</param>
    public SocketRouteAttribute(string route)
    {
        Route = route;
    }

    /// <summary>
    /// Gets the route for the socket connection.
    /// </summary>
    public string Route { get; private set; }
}
