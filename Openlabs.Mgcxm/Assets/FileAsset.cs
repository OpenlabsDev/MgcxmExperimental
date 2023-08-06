// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;
using System.Text.RegularExpressions;
using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Assets;

/// <summary>
/// Represents an asset stored in a file.
/// </summary>
public class FileAsset : ObjectAsset
{
    /// <summary>
    /// Checks whether the provided path is a local path.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <returns>True if the path is local, otherwise false.</returns>
    private bool IsPathLocal(string path) => !System.IO.Path.IsPathRooted(path);

    /// <summary>
    /// Loads the file asset using the provided arguments.
    /// </summary>
    /// <param name="arguments">The arguments required for loading the file asset. The first argument should be the file path.</param>
    public override async Task Load(object[] arguments)
    {
        // if the path is local, add "assets\" to the start
        var pathArg = (string)arguments[0];
        Directory.CreateDirectory(Constants.WorkingDirectory + "\\assets"); // init our assets path

        _path = (IsPathLocal(pathArg) ? "assets\\" : "") + pathArg;
        _guid = Hashing.MD5(_path); // init unique id

        _fileName = System.IO.Path.GetFileName(_path);
        _extension = System.IO.Path.GetExtension(_path);
    }

    /// <summary>
    /// Reads the content of the file asset as a string using UTF-8 encoding.
    /// </summary>
    /// <returns>The content of the file asset as a string.</returns>
    public string ReadString()
        => Encoding.UTF8.GetString(ReadBytes());

    /// <summary>
    /// Reads the content of the file asset as a byte array.
    /// </summary>
    /// <returns>The content of the file asset as a byte array.</returns>
    public byte[] ReadBytes() => File.ReadAllBytes(_path);

    /// <summary>
    /// Writes the provided string data to the file asset using UTF-8 encoding.
    /// </summary>
    /// <param name="data">The string data to write.</param>
    public void WriteString(string data)
        => WriteBytes(Encoding.UTF8.GetBytes(data));

    /// <summary>
    /// Writes the provided byte array data to the file asset.
    /// </summary>
    /// <param name="data">The byte array data to write.</param>
    public void WriteBytes(byte[] data) => File.WriteAllBytes(_path, data);

    /// <summary>
    /// Appends the provided string data to the file asset using UTF-8 encoding.
    /// </summary>
    /// <param name="data">The string data to append.</param>
    public void AppendString(string data)
        => WriteBytes(ReadBytes().Concat(Encoding.UTF8.GetBytes(data)).ToArray());

    /// <summary>
    /// Gets the file name of the asset.
    /// </summary>
    public string FileName => _fileName;

    /// <summary>
    /// Gets the file extension of the asset.
    /// </summary>
    public string Extension => _extension;

    /// <summary>
    /// Gets the file path of the asset.
    /// </summary>
    public string Path => _path;

    /// <summary>
    /// Gets the unique identifier (GUID) of the asset.
    /// </summary>
    public string Guid => _guid;

    private string _fileName;
    private string _extension;

    private string _path;
    private string _guid;
}