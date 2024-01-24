using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;

using System.Net;

/// <summary>
/// Defines a read-only Internet Protocol Address (IP Address).
/// </summary>
public class IpAddress : IIpAddress
{
    private readonly IPAddress _actualIpAddress;
    private readonly ushort _port;

    /// <inheritdoc/>
    public IPAddress ActualIpAddress => _actualIpAddress;

    /// <summary>
    /// Creates a new instance of an <see cref="IpAddress"/>.
    /// </summary>
    /// <param name="origin">The origin. (e.g., 1.1.1.1 or 2001:0db8:85a3:0000:0000:8a2e:0370:7334)</param>
    /// <param name="port">The port. (e.g., 20001)</param>
    public IpAddress(string origin, ushort port = 0)
    {
        _port = port;
        var originFormatted = AddressUtilities.FormatIpAddress(origin);

        if (_port == 0 && originFormatted.Contains(":"))
        {
            var originSegments = originFormatted.Split(':');

            var originSeg1 = originSegments[0]; // hostname
            var originSeg2 = originSegments[1]; // port

            originFormatted = originSeg1;
            _port = ushort.Parse(originSeg2);
        }

        Logger.Debug("trying to parse: {Origin}, export = {Port}", originFormatted, _port);
        if (IPAddress.TryParse(originFormatted, out var ipAddress))
        {
            _actualIpAddress = ipAddress;

            IsIPV4 = ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
            IsIPV6 = ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
        }
        else
            throw new ArgumentException("Invalid IP address format.", nameof(origin));
    }

    /// <summary>
    /// Creates an <see cref="IpAddress"/> from a System.Net.<see cref="IPAddress"/>
    /// </summary>
    public static implicit operator IpAddress(IPAddress ip)
    {
        return new IpAddress(ip.ToString());
    }

    /// <summary>
    /// Creates an <see cref="IpAddress"/> from a <see cref="string"/>
    /// </summary>
    public static implicit operator IpAddress(string ipString)
    {
        return new IpAddress(ipString);
    }

    /// <summary>
    /// Creates a well-formed IP address.
    /// </summary>
    public override string ToString()
    {
        string formedIp = _actualIpAddress.ToString();
        if (_port > 0)
            formedIp += $":{_port}";

        return formedIp;
    }

    /// <inheritdoc/>
    public string Origin => _actualIpAddress.ToString();

    /// <inheritdoc/>
    public ushort Port => _port;

    /// <inheritdoc/>
    public bool IsIPV4 { get; }

    /// <inheritdoc/>
    public bool IsIPV6 { get; }
}