// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

public static class AssetManager
{
    public static void LoadAll(AssetLoadRequest[] requests)
    {
        try
        {
            LogSink.Info($"Loading {requests.Length} asset(s) into memory");
            
            // load all objects into memory
            foreach (var tempRequest in requests)
            {
                if (!tempRequest.type.InheritsOrImplements(typeof(ObjectAsset)))
                    throw new InvalidDataException($"Cannot load a type that doesnt inherit ObjectAsset! Type = {tempRequest.type.Name}");

                _allLoadedAssets.Add((ObjectAsset)Activator.CreateInstance(tempRequest.type, tempRequest.args)!);
                LogSink.Trace($"Added '{tempRequest.type.Name}' to memory with {tempRequest.args.Length} argument(s)");
            }
        }
        catch (Exception ex)
        { LogSink.Exception("Cannot load all object assets", ex); }
    }

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

public class AssetLoadRequest
{
    public static implicit operator AssetLoadRequest((Type t, object[] a) valueTuple)
        => new AssetLoadRequest { type = valueTuple.t, args = valueTuple.a };
    
    public Type type;
    public object[] args;
}