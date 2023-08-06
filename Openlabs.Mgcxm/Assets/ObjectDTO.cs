// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents an abstract base class for Data Transfer Objects (DTOs).
/// </summary>
public abstract class ObjectDTO
{
    /// <summary>
    /// Gets the unique identifier for the DTO based on its type and property values.
    /// </summary>
    /// <returns>A string representing the unique identifier (MD5 hash) of the DTO.</returns>
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

    // A logger sink used for tracing DTO information.
    private static LoggerSink DtoSink = new LoggerSink("Dto");
}
