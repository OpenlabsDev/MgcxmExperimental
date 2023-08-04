// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;

namespace Openlabs.Mgcxm.Net;

public interface IIpAddress
{
    
    IPAddress ActualIpAddress { get; }
    
    string Origin { get; }
    ushort Port { get; }
}