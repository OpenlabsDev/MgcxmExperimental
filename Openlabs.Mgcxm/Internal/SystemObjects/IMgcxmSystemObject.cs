// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal.SystemObjects;

// system objects should properly allocate objects with the
// correct ID and object.

// system objects should properly deallocate objects with the
// correct ID.

// they should both properly have a constructor and finalizer

public interface IMgcxmSystemObject
{
    /// <summary>
    /// Get rid of this object from memory.
    /// </summary>
    void Trash();
    
    MgcxmId AllocatedId { get; }
}