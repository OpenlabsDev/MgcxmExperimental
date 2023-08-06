// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents a file asset containing a boolean flag, derived from <see cref="FileAsset"/>.
/// </summary>
public class FileFlagAsset : FileAsset
{
    /// <summary>
    /// Reads the boolean flag from the file asset.
    /// </summary>
    /// <returns>The value of the boolean flag read from the file asset.</returns>
    public bool ReadFlag()
    {
        try
        {
            return bool.Parse(ReadString());
        }
        catch { WriteString("false"); }

        return false;
    }

    /// <summary>
    /// Writes the boolean flag to the file asset.
    /// </summary>
    /// <param name="value">The value of the boolean flag to write.</param>
    public void WriteFlag(bool value)
    {
        WriteString(value.ToString().ToLower());
    }
}