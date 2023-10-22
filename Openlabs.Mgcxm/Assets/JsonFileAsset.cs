// Copr. (c) Nexus 2023. All rights reserved.

using Newtonsoft.Json;
using Openlabs.Mgcxm.Common.JsonMapping;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents a JSON file asset derived from <see cref="FileAsset"/>.
/// </summary>
public class JsonFileAsset : FileAsset
{
    /// <summary>
    /// Reads the content of the JSON file asset and deserializes it into the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the JSON into.</typeparam>
    /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the system fails to deserialize the contents into the specified type.</exception>
    public T ReadObject<T>()
    {
        try
        {
            AssetManager.LogSink.Debug("Reading JsonFileAsset({0}) as '{1}' type", Guid, typeof(T).Name);
            string contents = base.ReadString();
            AssetManager.LogSink.Debug(contents);

            var deserializedJson = JsonConvert.DeserializeObject<T>(contents)
                                 ?? throw new InvalidOperationException("Cannot read as JSON. The system was not able to deserialize the contents.");

            if (deserializedJson == null)
                AssetManager.LogSink.Warning("The JSON produced by JsonFileAsset({0}) is null! Was this supposed to happen?", Guid);

            return deserializedJson!;
        }
        catch (Exception ex)
        {
            Logger.Exception("Failed to deserialize DTO", ex);
            throw new InvalidOperationException("Cannot read as JSON. The system was not able to deserialize the contents.");
        }
    }
}