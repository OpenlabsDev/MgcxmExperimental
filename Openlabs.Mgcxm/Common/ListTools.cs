// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common;

/// <summary>
/// Provides utility methods for working with lists.
/// </summary>
public static class ListTools
{
    /// <summary>
    /// Creates a list of elements of type T with the specified capacity.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="action">A delegate that defines how to populate the list with elements.</param>
    /// <param name="capacity">The capacity of the list to be created.</param>
    /// <returns>A list of elements of type T with the specified capacity.</returns>
    public static List<T> Create<T>(CreateElementDelegate<T> action, int capacity)
    {
        List<T> list = new List<T>(capacity);
        for (int i = 0; i < capacity; i++)
            action(list, i);
        return list;
    }
}

/// <summary>
/// Represents a delegate that defines how to create elements in a list.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
/// <param name="list">The list to populate with elements.</param>
/// <param name="index">The index of the element to create.</param>
public delegate void CreateElementDelegate<T>(List<T> list, int index);
