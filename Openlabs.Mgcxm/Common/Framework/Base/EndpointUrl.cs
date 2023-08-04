// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Common.Framework;

public abstract class EndpointUrl
{
    public new abstract string ToString();
    public abstract void FromString(string url);

    public static TEndpointUrl FromString<TEndpointUrl>(string url)
        where TEndpointUrl : EndpointUrl
    {
        var epUrl = Activator.CreateInstance<TEndpointUrl>();
        epUrl.FromString(url);
        return epUrl;
    }
}