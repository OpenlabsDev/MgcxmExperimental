// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;

namespace Openlabs.Mgcxm.Net;

public interface IIpAddress
{
    /// <summary>
    /// The actual System object IP address.
    /// </summary>
    IPAddress ActualIpAddress { get; }

    /// <summary>
    /// The host of the IP address. (localhost / 1.1.1.1)
    /// </summary>
    string Origin { get; }

    /// <summary>
    /// The port of the IP address. (80 / 443)
    /// </summary>
    ushort Port { get; }
}