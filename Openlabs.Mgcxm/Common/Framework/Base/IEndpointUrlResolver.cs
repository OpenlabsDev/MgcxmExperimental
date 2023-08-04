// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.Framework;

public interface IEndpointUrlResolver
{
    MgcxmHttpEndpoint ResolveUrl(string url, MgcxmHttpRequest request);
}