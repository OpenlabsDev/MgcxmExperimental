// Copr. (c) Nexus 2023. All rights reserved.

using System.Reflection;
using Newtonsoft.Json;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;

namespace Openlabs.Mgcxm.Common.JsonMapping;

public static class MgcxmJsonObjectExtensions
{
    public static IMgcxmHttpResponseModifiable Json<T>(this IMgcxmHttpResponseModifiable mgcxmHrm, T obj)
    {
        var type = typeof(T);

        // create an instance and add headers, then serialize the object
        return mgcxmHrm.Header("X-Json-Object-Type", type.FullName)
                        .Header("X-Json-Version", $"{JsonConstants.Version.ToString()}(Openlabs.Mgcxm.Common.JsonMapping) like Newtonsoft.Json")
                        .Content(obj);
    }
} 