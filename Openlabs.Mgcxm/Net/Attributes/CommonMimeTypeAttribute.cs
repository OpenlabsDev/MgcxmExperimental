// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

[AttributeUsage(AttributeTargets.Field)]
public class CommonMimeTypeAttribute : Attribute
{
    public CommonMimeTypeAttribute(string mimeType, params string[] identifiers)
    {
        MimeType = mimeType;
        Identifiers = identifiers;
    }
    
    public CommonMimeTypeAttribute(string type, string subtype,  params string[] identifiers)
        : this(string.Format("{0}/{1}", type, subtype), identifiers) {}

    public bool Equals(string identifier)
    {
        if (Identifiers == null)
            return false;

        return Identifiers.Contains(identifier);
    }
    
    public string MimeType { get; private set; }
    public string[] Identifiers { get; private set; }
}