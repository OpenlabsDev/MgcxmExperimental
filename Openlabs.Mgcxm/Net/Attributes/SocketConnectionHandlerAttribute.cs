// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an attribute used to specify the type of socket connection handler for a socket.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SocketConnectionHandlerAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SocketConnectionHandlerAttribute"/> class with the specified handler type.
    /// </summary>
    /// <param name="type">The type of the socket connection handler.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the <paramref name="type"/> does not inherit or implement <see cref="MgcxmSocketConnectionHandler"/>.</exception>
    public SocketConnectionHandlerAttribute(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!type.InheritsOrImplements(typeof(MgcxmSocketConnectionHandler)))
            throw new InvalidOperationException("Cannot use a non MgcxmSocketConnectionHandler as a connection handler.");

        HandlerType = type;
    }

    /// <summary>
    /// Gets the type of the socket connection handler.
    /// </summary>
    public Type HandlerType { get; private set; }
}
