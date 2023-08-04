// Copr. (c) Nexus 2023. All rights reserved.

using Newtonsoft.Json;

namespace Openlabs.Mgcxm.Common;

public abstract class ServerDatabaseReader
{
    public abstract string ReadString(string fileName);
    public abstract int ReadInt(string fileName);
    public abstract T ReadGeneric<T>(string fileName);
    
    public abstract void WriteString(string fileName, string content);
    public abstract void WriteInt(string fileName, int content);
    public abstract void WriteGeneric<T>(string fileName, T content);
    
    public abstract void AppendString(string fileName, string content);

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

public enum ServerDatabaseFileType
{
    Int,
    String,
    Generic
}