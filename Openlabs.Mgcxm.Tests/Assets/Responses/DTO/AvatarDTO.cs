// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Common.JsonMapping;

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class AvatarDTO : ObjectDTO
{
    public string OutfitSelections { get; set; }
    public string HairColor { get; set; } = Guid.Empty.ToString();
    public string SkinColor { get; set; } = Guid.Empty.ToString();
}