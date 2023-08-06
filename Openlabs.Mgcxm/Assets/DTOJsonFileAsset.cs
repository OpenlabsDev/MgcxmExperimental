// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Common.JsonMapping;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents a file asset containing a JSON serialized object of type <typeparamref name="TDtoType"/>,
/// derived from <see cref="JsonFileAsset"/>.
/// </summary>
/// <typeparam name="TDtoType">The type of the DTO object.</typeparam>
public class DTOJsonFileAsset<TDtoType> : JsonFileAsset
{
    /// <summary>
    /// Reads the DTO object from the JSON file asset.
    /// </summary>
    /// <returns>The deserialized DTO object.</returns>
    public virtual TDtoType ReadDto()
    {
        return ReadObject<TDtoType>();
    }

    /// <summary>
    /// Writes the DTO object to the JSON file asset.
    /// </summary>
    /// <param name="dtoType">The DTO object to write.</param>
    public virtual void WriteDto(TDtoType dtoType)
    {
        WriteString(Json.ToJson(dtoType));
    }
}