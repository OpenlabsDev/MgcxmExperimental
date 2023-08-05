// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// An enumeration of all supported HTTP methods.
/// </summary>
public enum HttpMethods
{
    /// <summary>
    /// Represents an unknown or unsupported HTTP method.
    /// </summary>
    UNKNOWN,

    /// <summary>
    /// Represents an HTTP GET method used to retrieve data from the server.
    /// </summary>
    GET,

    /// <summary>
    /// Represents an HTTP POST method used to submit data to be processed by the server.
    /// </summary>
    POST,

    /// <summary>
    /// Represents an HTTP PUT method used to update existing data on the server.
    /// </summary>
    PUT,

    /// <summary>
    /// Represents an HTTP HEAD method used to request the headers of an HTTP response.
    /// </summary>
    HEAD,

    /// <summary>
    /// Represents an HTTP DELETE method used to request the deletion of a resource on the server.
    /// </summary>
    DELETE
}
