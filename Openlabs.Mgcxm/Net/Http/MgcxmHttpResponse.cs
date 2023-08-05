// Copr. (c) Nexus 2023. All rights reserved.

using System.Net;
using System.Text;
using Openlabs.Mgcxm.Common.JsonMapping;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Internal.SystemObjects;
using Random = Openlabs.Mgcxm.Common.Random;

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents an HTTP response for use in MgcxmSocketListener.
/// </summary>
public sealed class MgcxmHttpResponse : IMgcxmHttpResponseModifiable, IMgcxmHttpResponseTransferable
{
    // Factory method to create a new instance of MgcxmHttpResponse.
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

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable Status(HttpStatusCodes statusCode)
    {
        _statusCode = statusCode;
        return this;
    }

    /// <inheritdoc/>
    public int GetStatus()
    {
        return (int)_statusCode;
    }

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable Header(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, byte[] data)
    {
        _contentType = HttpContentTypeHelper.ResolveValue(contentType);
        _responseData = data;
        return this;
    }

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, string data)
    {
        return Content(contentType, Encoding.UTF8.GetBytes(data));
    }

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable Content<T>(T data)
    {
        return Content(HttpContentTypes.Json, data.ToJson());
    }

    /// <inheritdoc/>
    public IMgcxmHttpResponseModifiable File(string path)
    {
        if (!System.IO.File.Exists(path))
            throw new FileNotFoundException("The specified file was not found.", path);

        _responseData = System.IO.File.ReadAllBytes(path);
        return this;
    }

    #endregion

    #region IMgcxmHttpResponseTransferable

    /// <inheritdoc/>
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

    /// <summary>
    /// Gets a value indicating whether the HTTP response status code indicates success (2xx).
    /// </summary>
    public bool IsSuccessStatusCode => ((int)_statusCode >= 200) && ((int)_statusCode <= 299);

    /// <summary>
    /// Gets a value indicating whether the HTTP response status code indicates an error (non-2xx).
    /// </summary>
    public bool IsErrorStatusCode => !IsSuccessStatusCode;

    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public HttpStatusCodes StatusCode => _statusCode;

    /// <summary>
    /// Gets the error message associated with the response.
    /// </summary>
    public string ErrorMessage => _errorMessage;

    /// <summary>
    /// Gets the content type of the response.
    /// </summary>
    public string ContentType => _contentType;

    /// <summary>
    /// Gets the headers of the response.
    /// </summary>
    public Dictionary<MgcxmString, MgcxmString> Headers => _headers;

    /// <summary>
    /// Gets the response data as a byte array.
    /// </summary>
    public byte[] ResponseData => _responseData;

    /// <summary>
    /// Gets the MgcxmId of the owner of the response.
    /// </summary>
    public MgcxmId OwnerId => _ownerId;

    private HttpStatusCodes _statusCode;
    private string _errorMessage;
    private string _contentType;
    private Dictionary<MgcxmString, MgcxmString> _headers;
    private byte[] _responseData;
    private MgcxmId _ownerId;
}

/// <summary>
/// Represents an interface for modifying an HTTP response.
/// </summary>
public interface IMgcxmHttpResponseModifiable
{
    /// <summary>
    /// Sets the HTTP status code of the response.
    /// </summary>
    IMgcxmHttpResponseModifiable Status(HttpStatusCodes statusCode);

    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    int GetStatus();

    /// <summary>
    /// Sets a header in the response.
    /// </summary>
    IMgcxmHttpResponseModifiable Header(string key, string value);

    /// <summary>
    /// Sets the content of the response with a byte array and content type.
    /// </summary>
    IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, byte[] data);

    /// <summary>
    /// Sets the content of the response with a string data and content type.
    /// </summary>
    IMgcxmHttpResponseModifiable Content(HttpContentTypes contentType, string data);

    /// <summary>
    /// Sets the content of the response by serializing an object to JSON and setting content type as JSON.
    /// </summary>
    IMgcxmHttpResponseModifiable Content<T>(T data);

    /// <summary>
    /// Sets the content of the response from a file on disk.
    /// </summary>
    IMgcxmHttpResponseModifiable File(string path);
}

/// <summary>
/// Represents an interface for transferring an HTTP response to a HttpListenerResponse.
/// </summary>
public interface IMgcxmHttpResponseTransferable
{
    /// <summary>
    /// Transfers the response to a HttpListenerResponse.
    /// </summary>
    Task Transfer(HttpListenerResponse response);
}