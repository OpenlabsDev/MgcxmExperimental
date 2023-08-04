// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common;

public static class ListTools
{
    public static List<T> Create<T>(CreateElementDelegate<T> action, int capacity)
    {
        List<T> list = new List<T>(capacity);
        for (int i = 0; i < capacity; i++)
            action(list, i);
        return list;
    }
}

public delegate void CreateElementDelegate<T>(List<T> list, int index);