// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Common.JsonMapping;

namespace Openlabs.Mgcxm.Assets;

public class DTOJsonFileAsset<TDtoType> : JsonFileAsset
{
    public virtual TDtoType ReadDto()
    {
        return ReadObject<TDtoType>();
    }

    public virtual void WriteDto(TDtoType dtoType)
    {
        WriteString(Json.ToJson(dtoType));
    }
}