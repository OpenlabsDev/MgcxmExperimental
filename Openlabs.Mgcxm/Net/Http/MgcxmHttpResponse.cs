// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using System.Text;
using Openlabs.Mgcxm.Common.JsonMapping;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Internal.Random;

namespace Openlabs.Mgcxm.Net;

public sealed class MgcxmHttpResponse : IMgcxmHttpResponseModifiable, IMgcxmHttpResponseTransferable
{
    internal static MgcxmHttpResponse New(
        HttpStatusCodes statusCode, 
        string errorMessage, 
        Dictionary<MgcxmString, MgcxmString> headers, 
        byte[] response, 
        MgcxmId owner)
    {
        return new MgcxmHttpResponse
        {
            _statusCode = statusCode,
            _errorMessage = errorMessage,
            _headers = headers,
            _responseData = response,
            _ownerId = owner
        };
    }

    #region IMgcxmHttpResponseModifiable

    public IMgcxmHttpResponseModifiable Status(HttpStatusCodes statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    public int GetStatus()
    {
        return (int)_statusCode;
    }

    public IMgcxmHttpResponseModifiable Header(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    public IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, byte[] data)
    {
        _contentType = HttpContentTypeHelper.ResolveValue(contentType);
        _responseData = data;
        return this;
    }

    public IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, string data)
    {
        return Content(contentType, Encoding.UTF8.GetBytes(data));
    }

    public IMgcxmHttpResponseModifiable Content<T>(T data)
    {
        return Content(HttpContentTypes.Json, data.ToJson());
    }

    public IMgcxmHttpResponseModifiable File(string path)
    {
        if (!System.IO.File.Exists(path))
            throw new FileNotFoundException("The specified file was not found.", path);

        _responseData = System.IO.File.ReadAllBytes(path);
        return this;
    }

    #endregion

    #region IMgcxmHttpResponseTransferable

    public async Task Transfer(HttpListenerResponse response)
    {
        response.StatusCode = (int)_statusCode;
        foreach (var headerKvp in _headers)
            response.Headers.Set(headerKvp.Key, headerKvp.Value);

        response.ContentType = _contentType;
        response.ContentLength64 = _responseData.Length;
        
        var stream = response.OutputStream;
        await stream.WriteAsync(_responseData, 0, _responseData.Length);
        stream.Close();
    }

    #endregion

    public bool IsSuccessStatusCode
    {
        get
        {
            int statusCode = (int)_statusCode;
            return statusCode >= 200 && statusCode <= 299;
        }
    }

    public bool IsErrorStatusCode => !IsSuccessStatusCode;
    
    public HttpStatusCodes StatusCode => _statusCode;
    public string ErrorMessage => _errorMessage;
    public string ContentType => _contentType;
    public Dictionary<MgcxmString, MgcxmString> Headers => _headers;
    public byte[] ResponseData => _responseData;
    public MgcxmId OwnerId => _ownerId;

    private HttpStatusCodes _statusCode;
    private string _errorMessage;
    private string _contentType;
    private Dictionary<MgcxmString, MgcxmString> _headers;
    private byte[] _responseData;
    private MgcxmId _ownerId;
}

public interface IMgcxmHttpResponseModifiable
{
    IMgcxmHttpResponseModifiable Status(HttpStatusCodes statusCode);
    int GetStatus();
    
    IMgcxmHttpResponseModifiable Header(string key, string value);
    
    IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, byte[] data);
    IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, string data);
    IMgcxmHttpResponseModifiable Content<T>(T data);
    
    IMgcxmHttpResponseModifiable File(string path);
}

public interface IMgcxmHttpResponseTransferable
{
    Task Transfer(HttpListenerResponse response);
}