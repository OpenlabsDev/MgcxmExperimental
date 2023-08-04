// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Common.JsonMapping;

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class RecRoomConfigDTO : ObjectDTO
{
    public string MessageOfTheDay { get; set; }
    public string CdnBaseUri { get; set; }
    public MatchmakingParamsDTO MatchmakingParams { get; set; }
    public List<LevelProgressionIndexDTO> LevelProgressionMaps { get; set; }
    public List<List<ObjectiveDTO>> DailyObjectives { get; set; }
    public List<SettingDTO> ConfigTable { get; set; }
    public PhotonConfigDTO PhotonConfig { get; set; }
}

public class MatchmakingParamsDTO : ObjectDTO
{
    public float PreferFullRoomsFrequency { get; set; }
    public float PreferEmptyRoomsFrequency { get; set; }
}

public class LevelProgressionIndexDTO : ObjectDTO
{
    public LevelProgressionIndexDTO(int level)
    {
        Level = level;
        RequiredXp = (int)(level * 2 * 2.5 * 3);
    }
    
    public int Level { get; set; }
    public int RequiredXp { get; set; }
}

public class PhotonConfigDTO : ObjectDTO
{
    public string CloudRegion { get; set; }
    public bool CrcCheckEnabled { get; set; }
}

public enum PhotonRegion
{
    eu,
    us,
    asia,
    jp,
    au = 5,
    usw,
    sa,
    cae,
    kr,
    @in,
    none = 4
}