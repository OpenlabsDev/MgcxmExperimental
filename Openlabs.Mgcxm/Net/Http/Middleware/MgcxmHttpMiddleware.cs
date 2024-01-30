using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Net.Http.Middleware
{
    public abstract class MgcxmHttpMiddleware : IMgcxmHttpMiddleware
    {
        /// <inheritdoc/>
        public abstract MiddlewareResult OnRequestMade(MgcxmHttpRequest request, MgcxmHttpResponse response);
    }
}
