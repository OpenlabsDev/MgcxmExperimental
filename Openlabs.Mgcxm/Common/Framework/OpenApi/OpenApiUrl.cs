// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Internal;

namespace Openlabs.Mgcxm.Common.Framework;

public class OpenApiUrl : EndpointUrl
{
    public OpenApiUrl() {}
    public OpenApiUrl(string controller, uint version, params string[] subParts)
    {
        _controller = controller;
        _version = version;
        _subParts = subParts;
    }

    public static implicit operator string(OpenApiUrl apiUrl)
        => apiUrl.ToString();

    public override string ToString()
    {
        string format = string.Format("/api/{0}/v{1}", _controller, _version);
        if (_subParts.Length > 0)
            format += string.Format("/{0}", string.Join("/", _subParts));

        return format;
    }

    public override void FromString(string url)
    {
        // /api/{_controller}/v{_version}/{_subParts}

        try
        {
            // remove the api path
            string nUrl = url.Remove(0, 5);

            // get controller
            string[] parts = nUrl.Split("/");
            _controller = parts[0];

            // parse version
            if (!uint.TryParse(parts[1].Replace("v", string.Empty), out uint version))
                throw new OpenApiUrlFromStringException("Cannot parse version. Given URL = " + url);
            _version = version;

            // get subparts
            List<string> subparts = new List<string>();
            for (int i = 2; i < parts.Length; i++)
                subparts.Add(parts[i]);
            _subParts = subparts.ToArray();
        }
        catch (Exception exception)
        {
            Logger.Error($"Failed to parse OpenApiUrl: ({exception.GetType().GetSafeName()}): {exception}");
        }
    }

    public string Controller => _controller;
    public uint Version => _version;
    public string[] SubParts => _subParts;

    private string _controller;
    private uint _version;
    private string[] _subParts;

    public static readonly OpenApiUrl Empty = new("empty", 0);
}

public class OpenApiUrlFromStringException : Exception
{
    public OpenApiUrlFromStringException()
    {
    }

    public OpenApiUrlFromStringException(string message) : base(message)
    {
    }

    public OpenApiUrlFromStringException(string message, Exception inner) : base(message, inner)
    {
    }
}