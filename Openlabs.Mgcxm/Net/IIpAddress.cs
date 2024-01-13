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

    /// <summary>
    /// Returns if the IP address is IPv4 (14.193.195.140)
    /// </summary>
    bool IsIPV4 { get; }

    /// <summary>
    /// Returns if the IP address is IPv6 (FE80:CD00 / FE80:CD00:0000:0CDE:1257:0000:211E:729C)
    /// </summary>
    bool IsIPV6 { get; }
}