using Openlabs.Mgcxm.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Internal
{
    internal static class Events
    {
        public static Action<MgcxmHttpListener, MgcxmHttpRequest> OnHttpRequestMade = (_, _) => { };
        public static Action<MgcxmSocketListener, MgcxmSocketRoute> OnWsRequestMade = (_, _) => { };
    }
}
