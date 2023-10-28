// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;
using Openlabs.Mgcxm.Common;
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
            _ownerId = owner,
            _requestId = Uuid.Create()
        };
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

    /// <summary>
    /// Gets the unique ID of the request.
    /// </summary>
    public Uuid Id => _requestId;

    private HttpMethods _httpMethod;
    private Uri _baseAddress = null!;
    private string _uri = null!;
    private HttpContentTypes _contentType;
    private byte[] _rawBodyData = null!;
    private Dictionary<string, string> _form = null!;
    private Dictionary<string, string> _headers = null!;
    private Dictionary<string, string> _query = null!;
    private MgcxmId _ownerId;
    private Uuid _requestId;
}
