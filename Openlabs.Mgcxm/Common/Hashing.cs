// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;

namespace Openlabs.Mgcxm.Common;

/// <summary>
/// Provides methods for generating hash codes using various hashing algorithms.
/// </summary>
public static class Hashing
{
    /// <summary>
    /// Computes the MD5 hash value for the input string.
    /// </summary>
    /// <param name="input">The input string to compute the MD5 hash for.</param>
    /// <returns>The MD5 hash value as a hexadecimal string.</returns>
    public static string MD5(string input)
        => BitConverter.ToString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);

    /// <summary>
    /// Computes the SHA-1 hash value for the input string.
    /// </summary>
    /// <param name="input">The input string to compute the SHA-1 hash for.</param>
    /// <returns>The SHA-1 hash value as a hexadecimal string.</returns>
    public static string SHA1(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA1.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);

    /// <summary>
    /// Computes the SHA-256 hash value for the input string.
    /// </summary>
    /// <param name="input">The input string to compute the SHA-256 hash for.</param>
    /// <returns>The SHA-256 hash value as a hexadecimal string.</returns>
    public static string SHA256(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);

    /// <summary>
    /// Computes the SHA-384 hash value for the input string.
    /// </summary>
    /// <param name="input">The input string to compute the SHA-384 hash for.</param>
    /// <returns>The SHA-384 hash value as a hexadecimal string.</returns>
    public static string SHA384(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA384.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);

    /// <summary>
    /// Computes the SHA-512 hash value for the input string.
    /// </summary>
    /// <param name="input">The input string to compute the SHA-512 hash for.</param>
    /// <returns>The SHA-512 hash value as a hexadecimal string.</returns>
    public static string SHA512(string input)
        => BitConverter.ToString(System.Security.Cryptography.SHA512.HashData(Encoding.UTF8.GetBytes(input))).Replace("-", string.Empty);
}