// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// A static class responsible for managing and loading various assets.
/// </summary>
public static class AssetManager
{
    /// <summary>
    /// Loads all specified asset requests into memory.
    /// </summary>
    /// <param name="requests">An array of asset load requests.</param>
    public static void LoadAll(AssetLoadRequest[] requests)
    {
        try
        {
            LogSink.Info($"Loading {requests.Length} asset(s) into memory");

            // load all objects into memory
            foreach (var tempRequest in requests)
            {
                if (!tempRequest.type.InheritsOrImplements(typeof(ObjectAsset)))
                    throw new InvalidDataException($"Cannot load a type that doesn't inherit ObjectAsset! Type = {tempRequest.type.Name}");

                _allLoadedAssets.Add((ObjectAsset)Activator.CreateInstance(tempRequest.type, tempRequest.args)!);
                LogSink.Trace($"Added '{tempRequest.type.Name}' to memory with {tempRequest.args.Length} argument(s)");
            }
        }
        catch (Exception ex)
        {
            LogSink.Exception("Cannot load all object assets", ex);
        }
    }

    /// <summary>
    /// Asynchronously loads and initializes an object asset of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object asset to load.</typeparam>
    /// <param name="arguments">The arguments required for asset initialization.</param>
    /// <returns>The loaded object asset or null if there was an error during loading.</returns>
    public static async Task<T> Load<T>(params object[] arguments)
        where T : ObjectAsset
    {
        try
        {
            T obj = (T)(ObjectAsset)Activator.CreateInstance(typeof(T))!;
            await obj.Load(arguments);

            _allLoadedAssets.Add(obj);

            return obj;
        }
        catch (Exception ex)
        {
            LogSink.Exception("Cannot load object asset", ex);
            return null;
        }
    }

    private static List<ObjectAsset> _allLoadedAssets = new();
    public static readonly LoggerSink LogSink = new("AstMgr");
}

/// <summary>
/// Represents an asset load request specifying the asset type and its initialization arguments.
/// </summary>
public class AssetLoadRequest
{
    /// <summary>
    /// Implicitly converts a value tuple of asset type and initialization arguments to an asset load request.
    /// </summary>
    /// <param name="valueTuple">The value tuple containing asset type and initialization arguments.</param>
    /// <returns>An asset load request.</returns>
    public static implicit operator AssetLoadRequest((Type t, object[] a) valueTuple)
        => new AssetLoadRequest { type = valueTuple.t, args = valueTuple.a };

    /// <summary>
    /// The type of the asset to load.
    /// </summary>
    public Type type;

    /// <summary>
    /// The arguments required for initializing the asset.
    /// </summary>
    public object[] args;
}
