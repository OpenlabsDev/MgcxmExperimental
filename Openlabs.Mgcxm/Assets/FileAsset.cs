// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;
using System.Text.RegularExpressions;
using Openlabs.Mgcxm.Common;

namespace Openlabs.Mgcxm.Assets;

public class FileAsset : ObjectAsset
{
    public override async Task Load(object[] arguments)
    {
        _path = "assets\\" + (string)arguments[0];
        _guid = Hashing.MD5(_path);
        
        _fileName = System.IO.Path.GetFileName(_path);
        _extension = System.IO.Path.GetExtension(_path);
    }

    public string ReadString()
        => Encoding.UTF8.GetString(ReadBytes());

    public byte[] ReadBytes() => File.ReadAllBytes(_path);

    public void WriteString(string data)
        => WriteBytes(Encoding.UTF8.GetBytes(data));

    public void WriteBytes(byte[] data) => File.WriteAllBytes(_path, data);

    public void AppendString(string data)
        => WriteBytes(ReadBytes().Concat(Encoding.UTF8.GetBytes(data)).ToArray());

    public string FileName => _fileName;
    public string Extension => _extension;
    public string Path => _path;
    public string Guid => _guid;

    private string _fileName;
    private string _extension;
    
    private string _path;
    private string _guid;
}