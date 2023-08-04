// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Net;

public class SocketConnectionHandlerAttribute : Attribute
{
    public SocketConnectionHandlerAttribute(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));
        
        if (!type.InheritsOrImplements(typeof(MgcxmSocketConnectionHandler)))
            throw new InvalidOperationException(
                "Cannot use a non MgcxmSocketConnectionHandler as a connection handler.");

        HandlerType = type;
    }
    
    public Type HandlerType { get; private set; }
}