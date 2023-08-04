// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;

namespace Openlabs.Mgcxm.Internal;

public static class AttributeResolver
{
    public static void ResolveAllType<T>(Type typeOfAttribute, Action<Type, T> resolve)
        where T : Attribute
    {
        var assembly = Assembly.GetAssembly(typeof(AttributeResolver));

        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute(typeOfAttribute);
            if (attribute != null)
                resolve(type, (T)(object)attribute);
        }
    }
    
    public static void ResolveAllMethod<T>(Type classType, Type typeOfAttribute, Action<MethodInfo, T> resolve)
        where T : Attribute
    {
        foreach (var method in classType.GetMethods())
        {
            var attribute = method.GetCustomAttribute(typeOfAttribute);
            if (attribute != null)
                resolve(method, (T)(object)attribute);
        }
    }
}