using Openlabs.Mgcxm.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgcxm.Test
{
    internal class HttpServerTest : MgcxmHttpListener
    {
        public HttpServerTest() : base("localhost:5000")
        {

        }

        [HttpEndpoint(HttpMethods.GET, "/")]
        public void Index(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                    .Content(HttpContentTypes.PlainText, "Index")
                    .Finish();
        }

        [HttpEndpoint(HttpMethods.GET, "/forceError")]
        public void ForceError(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            throw new Exception("Error forced by client");
        }
    }
}
