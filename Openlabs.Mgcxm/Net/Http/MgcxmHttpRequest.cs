// Copr. (c) Nexus 2023. All rights reserved.

using System.ComponentModel;
using System.Text;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Net;


/// <summary>
/// Represents an HTTP request for use in MgcxmSocketListener.
/// </summary>
public sealed class MgcxmHttpRequest
{
    // Factory method to create a new instance of MgcxmHttpRequest.
    internal static MgcxmHttpRequest New(
        HttpMethods method,
        Uri baseAddress,
        string uri,
        HttpContentTypes contentType,
        byte[] bodyData,
        Dictionary<string, string> form,
        Dictionary<string, string> headers,
        Dictionary<string, string> query,
        MgcxmId owner)
    {
        return new MgcxmHttpRequest
        {
            _httpMethod = method,
            _baseAddress = baseAddress,
            _uri = uri,
            _contentType = contentType,
            _rawBodyData = bodyData,
            _form = form,
            _headers = headers,
            _query = query,
            _ownerId = owner
        };
    }

    /// <summary>
    /// Gets the specified segment from the URI path.
    /// </summary>
    /// <param name="segmentIndex">The index of the segment to retrieve (0-based).</param>
    /// <returns>
    /// The value of the specified URI segment if it exists; otherwise, an empty string.
    /// </returns>
    public string GetUrlSegment(int segmentIndex)
    {
        var urlSegments = _uri.Split("/");

        if (urlSegments.Length > segmentIndex) 
            return string.Empty;
        return urlSegments[segmentIndex] ?? string.Empty;
    }

    /// <summary>
    /// Gets the specified segment from the URI path.
    /// </summary>
    /// <param name="segmentIndex">The index of the segment to retrieve (0-based).</param>
    /// <returns>
    /// The value of the specified URI segment if it exists; otherwise, an empty string.
    /// </returns>
    public T GetUrlSegment<T>(int segmentIndex)
    {
        var urlSegments = _uri.Split("/");
        var typeConverter = TypeDescriptor.GetConverter(typeof(T));

        if (typeConverter == null || urlSegments.Length > segmentIndex)
            return default;
        return (T)typeConverter.ConvertFromString(urlSegments[segmentIndex]);
    }

    /// <summary>
    /// Gets the value of a specific header from the request.
    /// </summary>
    /// <param name="headerName">The name of the header.</param>
    /// <returns>The value of the header, or an empty string if the header is not present.</returns>
    public string GetHeader(string headerName)
    {
        if (_headers.TryGetValue(headerName, out var value))
        {
            return value ?? string.Empty;
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets the value of a specific query parameter from the request.
    /// </summary>
    /// <param name="parameterName">The name of the query parameter.</param>
    /// <returns>The value of the query parameter, or an empty string if the parameter is not present.</returns>
    public string GetQueryParameter(string parameterName)
    {
        if (_query.TryGetValue(parameterName, out var value))
        {
            return value ?? string.Empty;
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets a formatted string representing the request headers.
    /// </summary>
    /// <returns>A formatted string with key-value pairs of headers.</returns>
    public string GetHeadersAsString()
    {
        StringBuilder headersString = new StringBuilder();

        foreach (var header in _headers)
        {
            headersString.AppendLine($"{header.Key}: {header.Value}");
        }

        return headersString.ToString();
    }

    /// <summary>
    /// Gets a formatted string representing the request query parameters.
    /// </summary>
    /// <returns>A formatted string with key-value pairs of query parameters.</returns>
    public string GetQueryParametersAsString()
    {
        StringBuilder queryParametersString = new StringBuilder();

        foreach (var queryParam in _query)
        {
            queryParametersString.AppendLine($"{queryParam.Key}: {queryParam.Value}");
        }

        return queryParametersString.ToString();
    }

    /// <summary>
    /// Gets the HTTP method of the request.
    /// </summary>
    public HttpMethods HttpMethod => _httpMethod;

    /// <summary>
    /// Gets the base address of the request.
    /// </summary>
    public Uri BaseAddress => _baseAddress;

    /// <summary>
    /// Gets the URI of the request.
    /// </summary>
    public string Uri => _uri;

    /// <summary>
    /// Gets the content type of the request.
    /// </summary>
    public HttpContentTypes ContentType => _contentType;

    /// <summary>
    /// Gets the raw body data of the request as a byte array.
    /// </summary>
    public byte[] RawBodyData => _rawBodyData;

    /// <summary>
    /// Gets the body data of the request as a string (decoded from UTF-8).
    /// </summary>
    public string Body => Encoding.UTF8.GetString(_rawBodyData);

    /// <summary>
    /// Gets the form data of the request as a dictionary of key-value pairs.
    /// </summary>
    public Dictionary<string, string> Form => _form;

    /// <summary>
    /// Gets the headers of the request as a dictionary of key-value pairs.
    /// </summary>
    public Dictionary<string, string> Headers => _headers;

    /// <summary>
    /// Gets the query parameters of the request as a dictionary of key-value pairs.
    /// </summary>
    public Dictionary<string, string> Query => _query;

    /// <summary>
    /// Gets the MgcxmId of the owner of the request.
    /// </summary>
    public MgcxmId OwnerId => _ownerId;

    private HttpMethods _httpMethod;
    private Uri _baseAddress = null!;
    private string _uri = null!;
    private HttpContentTypes _contentType;
    private byte[] _rawBodyData = null!;
    private Dictionary<string, string> _form = null!;
    private Dictionary<string, string> _headers = null!;
    private Dictionary<string, string> _query = null!;
    private MgcxmId _ownerId;
}
