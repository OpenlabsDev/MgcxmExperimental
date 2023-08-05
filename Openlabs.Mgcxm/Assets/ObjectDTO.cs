// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

public abstract class ObjectDTO
{
    public string GetDtoId()
    {
        var type = GetType();
        string str = $"{type.FullName}({GetHashCode()})|";
        
        foreach (var member in type.GetProperties())
        {
            var value = member.GetValue(this);
            str += $"({member.DeclaringType.Name}){member.Name}=({value.GetType().Name}){value}|";
        }
        DtoSink.Trace(str);
        
        return Hashing.MD5(str);
    }

    private static LoggerSink DtoSink = new("Dto");
}