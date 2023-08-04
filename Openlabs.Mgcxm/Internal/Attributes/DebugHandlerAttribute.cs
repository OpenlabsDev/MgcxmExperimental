// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal;

public sealed class DebugHandlerAttribute : Attribute
{
    public DebugHandlerAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; private set; }
}