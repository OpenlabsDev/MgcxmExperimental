// Copr. (c) Nexus 2023. All rights reserved.

using Newtonsoft.Json;

namespace Openlabs.Mgcxm.Common;

/// <summary>
/// An abstract class for reading and writing data from/to a server database.
/// </summary>
public abstract class ServerDatabaseReader
{
    /// <summary>
    /// Reads a string from the specified file in the server database.
    /// </summary>
    /// <param name="fileName">The name of the file to read.</param>
    /// <returns>The string content read from the file.</returns>
    public abstract string ReadString(string fileName);

    /// <summary>
    /// Reads an integer from the specified file in the server database.
    /// </summary>
    /// <param name="fileName">The name of the file to read.</param>
    /// <returns>The integer content read from the file.</returns>
    public abstract int ReadInt(string fileName);

    /// <summary>
    /// Reads data of a generic type from the specified file in the server database.
    /// </summary>
    /// <typeparam name="T">The type of data to read.</typeparam>
    /// <param name="fileName">The name of the file to read.</param>
    /// <returns>The generic data read from the file.</returns>
    public abstract T ReadGeneric<T>(string fileName);

    /// <summary>
    /// Writes a string to the specified file in the server database.
    /// </summary>
    /// <param name="fileName">The name of the file to write.</param>
    /// <param name="content">The string content to write.</param>
    public abstract void WriteString(string fileName, string content);

    /// <summary>
    /// Writes an integer to the specified file in the server database.
    /// </summary>
    /// <param name="fileName">The name of the file to write.</param>
    /// <param name="content">The integer content to write.</param>
    public abstract void WriteInt(string fileName, int content);

    /// <summary>
    /// Writes data of a generic type to the specified file in the server database.
    /// </summary>
    /// <typeparam name="T">The type of data to write.</typeparam>
    /// <param name="fileName">The name of the file to write.</param>
    /// <param name="content">The generic data to write.</param>
    public abstract void WriteGeneric<T>(string fileName, T content);

    /// <summary>
    /// Appends a string to the specified file in the server database.
    /// </summary>
    /// <param name="fileName">The name of the file to append.</param>
    /// <param name="content">The string content to append.</param>
    public abstract void AppendString(string fileName, string content);

    /// <summary>
    /// Checks if the given content matches the specified file type in the server database.
    /// </summary>
    /// <param name="type">The type of file to check against.</param>
    /// <param name="content">The content to be checked.</param>
    /// <returns>True if the content matches the file type; otherwise, false.</returns>
    public bool IsContentTypeOf(ServerDatabaseFileType type, string content)
    {
        if (type == ServerDatabaseFileType.Int)
            return int.TryParse(content, out _);
        if (type == ServerDatabaseFileType.String)
        {
            try
            {
                object json = JsonConvert.DeserializeObject(content);
                if (json == null) return true;
                return false;
            }
            catch { return true; }
        }
        if (type == ServerDatabaseFileType.Generic)
        {
            try
            {
                object json = JsonConvert.DeserializeObject(content);
                if (json != null) return true;
                return false;
            }
            catch { return false; }
        }

        return false;
    }
}

/// <summary>
/// Represents the types of files in the server database.
/// </summary>
public enum ServerDatabaseFileType
{
    /// <summary>File containing integer data.</summary>
    Int,
    /// <summary>File containing string data.</summary>
    String,
    /// <summary>File containing generic data.</summary>
    Generic
}
