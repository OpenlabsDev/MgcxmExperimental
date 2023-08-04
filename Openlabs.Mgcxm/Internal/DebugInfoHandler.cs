// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal;

public abstract class DebugInfoHandler
{
    public abstract void Print(object target);
}

public static class DebugInfoHandlerExtensions
{
    public static void PrintDebugInfo<TOf>(this TOf obj)
    {
        AttributeResolver.ResolveAllType<DebugHandlerAttribute>(
            typeof(DebugHandlerAttribute), 
            (type, attr) =>
            {
                var objType = typeof(TOf);
                
                if (objType.GUID == attr.Type.GUID)
                {
                    var objHandler = (DebugInfoHandler)Activator.CreateInstance(type);
                    Logger.Trace($"=======================================================");
                    Logger.Trace($"Debug info data of {obj.GetType().Name} - 0x{objType.GUID.ToByteArray().GetHashCode():x8}");
                    objHandler.Print(obj);
                    Logger.Trace($"=======================================================");
                }
            });
    }
}