// Copr. (c) Nexus 2023. All rights reserved.

using Newtonsoft.Json;
using Openlabs.Mgcxm.Common.JsonMapping;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

public class JsonFileAsset : FileAsset
{
    public T ReadObject<T>()
    {
        AssetManager.LogSink.Debug("Reading JsonFileAsset({0}) as '{1}' type", Guid, typeof(T).Name);
        string contents = base.ReadString();
        AssetManager.LogSink.Debug(contents);
        
        T deserializedJson = contents.FromJson<T>() 
                             ?? throw new InvalidOperationException("Cannot read as JSON. The system was not able to deserialize the contents.");
        
        if (deserializedJson == null)
            AssetManager.LogSink.Warning("The JSON produced by JsonFileAsset({0}) is null! Was this supposed to happen?", Guid);

        return deserializedJson!;
    }
}