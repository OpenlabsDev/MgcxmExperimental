// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Defines a <c>Internet Protocol Address (IP Address)</c>, which is read-only.
/// </summary>
public class IpAddress : IIpAddress
{
    /// <summary>
    /// The actual System object IP address.
    /// </summary>
    public IPAddress ActualIpAddress => IPAddress.Parse(ToString());

    /// <summary>
    /// Creates a new instance of an <see cref="IpAddress"/>.
    /// </summary>
    /// <param name="origin">The origin. (e.g. 1.1.1.1)</param>
    /// <param name="port">The port. (e.g. 20001)</param>
    public IpAddress(string origin, ushort port = 0)
    {
        _origin = origin;
        _port = port;
    }

    public static implicit operator IpAddress(IPAddress ip)
    {
        string ipString = ip.ToString();
        string[] parts = ipString.Split(":");

        ushort port = 0;
        if (parts.Length > 1)
            port = ushort.Parse(parts[1]);

        return new IpAddress(parts[0], port);
    }
    
    public static implicit operator IpAddress(string ipString)
    {
        string[] parts = ipString.Split(":");

        ushort port = 0;
        if (parts.Length > 1)
            port = ushort.Parse(parts[1]);

        return new IpAddress(parts[0], port);
    }

    public static implicit operator IPAddress(IpAddress ip)
        => ip.ActualIpAddress;
    
    public override string ToString()
    {
        string formedIp = _origin;
        if (_port > 0)
            formedIp += $":{_port}";

        return formedIp;
    }

    /// <summary>
    /// The host of the IP address. (localhost / 1.1.1.1)
    /// </summary>
    public string Origin => _origin;

    /// <summary>
    /// The port of the IP address. (80 / 443)
    /// </summary>
    public ushort Port => _port;

    private string _origin;
    private ushort _port;
}