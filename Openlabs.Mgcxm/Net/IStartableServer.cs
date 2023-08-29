using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Net;

public interface IStartableServer
{
    void Start();
    void Stop();

    X509Certificate2 Certificate { get; }
    IpAddress Address { get; }
}
