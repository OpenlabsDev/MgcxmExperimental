// Copr. (c) Nexus 2023. All rights reserved.

using Microsoft.VisualBasic;

namespace Openlabs.Mgcxm.Internal;

/// <summary>
/// A simple way to identify an object allocated in Mgcxm space.
/// </summary>
public struct MgcxmId : IEquatable<MgcxmId>
{
    private MgcxmId(long id)
    { 
        _id = id;
        _children = new Dictionary<MgcxmId, object>();
    }

    public static implicit operator MgcxmId(long id) => new MgcxmId(id);
    public static implicit operator string(MgcxmId id) => id.Id.ToString();
    
    public bool Equals(MgcxmId other) => other._id == _id;
    
    public override bool Equals(object? obj) => obj is MgcxmId other && Equals(other);
    public override int GetHashCode() => (int)_id;

    public void Latch<T>(MgcxmId id, T owner) => _children.Add(id, owner);
    public void Unlatch(MgcxmId id) => _children.Remove(id);

    /// <summary>
    /// The ID of the object.
    /// </summary>
    public long Id => _id;
    
    /// <summary>
    /// The child objects of this ID.
    /// </summary>
    public IReadOnlyList<MgcxmId> Children => _children.Keys.ToList();

    private long _id;
    private Dictionary<MgcxmId, object> _children;
}

/// <summary>
/// Manages all objects with <see cref="MgcxmId"/> present.
/// </summary>
public static class MgcxmObjectManager
{
    /// <summary>
    /// Adds an object to the registry.
    /// </summary>
    /// <param name="id">The ID of the object to add.</param>
    /// <param name="obj">The object to add.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <exception cref="InvalidOperationException">Throws if the object ID given already exists in the registry.</exception>
    public static void Register<T>(MgcxmId id, T obj)
    {
        if (!_objects.ContainsKey(id))
        {
            Logger.Trace("Retrieving object of type '{TypeSafeName}' from registry. Id = {AllocatedId}", typeof(T).GetSafeName(), $"0x{id.Id:x8}");
            _objects.Add(id, obj!);
            
        }
        //else
        //    throw new InvalidOperationException(string.Format(ALLOCATE_OBJECT_ERROR_MESSAGE, (string)id));
    }
    
    /// <summary>
    /// Removes an object from the registry.
    /// </summary>
    /// <param name="id">The ID of the object to remove.</param>
    /// <exception cref="InvalidOperationException">Throws if the object ID given does not exist in the registry.</exception>
    public static void Deregister(MgcxmId id)
    {
        if (!Constants.CleanupItems) return;

        if (_objects.ContainsKey(id))
        {
            Logger.Trace("Removing object from registry. Id = {AllocatedId}", $"0x{id.Id:x8}");
            _objects.Remove(id);
        }
        //else
        //    throw new InvalidOperationException(string.Format(DEALLOCATE_OBJECT_ERROR_MESSAGE, (string)id));
    }

    /// <summary>
    /// Retrieves an object from the registry.
    /// </summary>
    /// <param name="id">The ID of the object.</param>
    /// <typeparam name="T">The type of object to retrieve.</typeparam>
    /// <exception cref="NullReferenceException">Throws when trying to retrieve a null object.</exception>
    /// <returns>A casted object retrieved from the registry.</returns>
    public static T Retrieve<T>(MgcxmId id)
    {
        if (!_objects.ContainsKey(id))
            return (T)(object)null!;

        var obj = _objects[id];
        if (!(obj is T))
            throw new InvalidCastException(string.Format(RETRIEVE_OBJECT_ERROR_MESSAGE, obj.GetType().Name, nameof(T)));

        Logger.Trace("Retrieving object of type '{TypeSafeName}' from registry. Id = {AllocatedId}", typeof(T).GetSafeName(), $"0x{id.Id:x8}");
        return (T)obj ?? throw new NullReferenceException("Tried to retrieve a null object.");
    }

    private static Dictionary<MgcxmId, object> _objects = new();

    private const string ALLOCATE_OBJECT_ERROR_MESSAGE = "Tried adding an already allocated object with the same ID to the registry! Id = {0}";
    private const string DEALLOCATE_OBJECT_ERROR_MESSAGE = "Tried removing a non-existing object from the registry! Id = {0}";
    private const string RETRIEVE_OBJECT_ERROR_MESSAGE = "Cannot cast the object into the given type. Expected '{0}', got '{1}'.";
}