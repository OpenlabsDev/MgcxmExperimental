// Copr. (c) Nexus 2023. All rights reserved.

using System.Text;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Internal.Random;

namespace Openlabs.Mgcxm.Net;


public sealed class MgcxmHttpRequest
{
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

    public HttpMethods HttpMethod => _httpMethod;
    public Uri BaseAddress => _baseAddress;
    public string Uri => _uri;
    public HttpContentTypes ContentType => _contentType;
    public byte[] RawBodyData => _rawBodyData;
    public string Body => Encoding.UTF8.GetString(_rawBodyData);
    public Dictionary<string, string> Form => _form;
    public Dictionary<string, string> Headers => _headers;
    public Dictionary<string, string> Query => _query;
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