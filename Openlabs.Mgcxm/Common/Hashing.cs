// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;

namespace Openlabs.Mgcxm.Common;

public static class Hashing
{
    public static string MD5(string input)
        => BitConverter.ToString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
    public static string SHA1(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA1.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
    public static string SHA256(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
    public static string SHA384(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA384.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
    public static string SHA512(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA512.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
}