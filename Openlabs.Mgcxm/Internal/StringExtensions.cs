// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Internal;

public static class StringExtensions
{
    public static string Truncate(this string str, int count, string end = "")
    {
        string sub = str.Length > count ? str.Substring(0, count) : str;
        return sub + end;
    }
}