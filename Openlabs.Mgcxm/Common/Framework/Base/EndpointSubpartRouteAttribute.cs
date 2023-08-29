using Openlabs.Mgcxm.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Common.Framework;

public sealed class EndpointSubpartRouteAttribute : Attribute
{
    public EndpointSubpartRouteAttribute(HttpMethods httpMethod, string route)
    {
        HttpMethod = httpMethod;
        Route = route;
    }

    public HttpMethods HttpMethod { get; set; }
    public string Route { get; set; }
}
