// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents an abstract base class for loading and managing object assets.
/// </summary>
public abstract class ObjectAsset
{
    /// <summary>
    /// Loads the object asset asynchronously using the specified arguments.
    /// </summary>
    /// <param name="arguments">An array of arguments required for loading the asset.</param>
    /// <returns>A task representing the asynchronous loading operation.</returns>
    public abstract Task Load(object[] arguments);
}
