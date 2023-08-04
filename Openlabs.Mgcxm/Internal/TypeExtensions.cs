// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal;

public static class TypeExtensions
{
    public static string GetSafeName(this Type type)
    {
        string typeName = type.Name;
        string genericParameters = string.Empty;

        if (type.IsGenericType)
        {
            typeName = typeName.Split("`")[0];
            var types = from x in type.GenericTypeArguments
                select x.Name;
            
            genericParameters = $"<{string.Join(",", types)}>";
        }

        return $"{typeName}{genericParameters}";
    }

    public static bool InheritsOrImplements(this Type child, Type parent)
    {
        parent = ResolveGenericTypeDefinition(parent);

        var currentChild = child.IsGenericType
            ? child.GetGenericTypeDefinition()
            : child;

        while (currentChild != typeof (object))
        {
            if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                return true;

            currentChild = currentChild.BaseType != null
                           && currentChild.BaseType.IsGenericType
                ? currentChild.BaseType.GetGenericTypeDefinition()
                : currentChild.BaseType;

            if (currentChild == null)
                return false;
        }
        return false;
    }

    private static bool HasAnyInterfaces(Type parent, Type child)
    {
        return child.GetInterfaces()
            .Any(childInterface =>
            {
                var currentInterface = childInterface.IsGenericType
                    ? childInterface.GetGenericTypeDefinition()
                    : childInterface;

                return currentInterface == parent;
            });
    }

    private static Type ResolveGenericTypeDefinition(Type parent)
    {
        var shouldUseGenericType = true;
        if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
            shouldUseGenericType = false;

        if (parent.IsGenericType && shouldUseGenericType)
            parent = parent.GetGenericTypeDefinition();
        return parent;
    }
}