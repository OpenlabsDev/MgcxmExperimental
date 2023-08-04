// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class SettingDTO : ObjectDTO
{
    public SettingDTO(string key, string value)
    {
        Key = key;
        Value = value;
    }
    
    public string Key { get; set; }
    public string Value { get; set; }
}