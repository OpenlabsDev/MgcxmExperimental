// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Assets;

public class FileFlagAsset : FileAsset
{
    public bool ReadFlag()
    {
        try
        {
            return bool.Parse(ReadString());
        }
        catch { WriteString("false"); }

        return false;
    }

    public void WriteFlag(bool value)
    {
        WriteString(value.ToString().ToLower());
    }
}