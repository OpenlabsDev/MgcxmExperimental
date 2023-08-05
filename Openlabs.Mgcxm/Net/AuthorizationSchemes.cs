// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net;

/// <summary>
/// Represents different types of authorization schemes.
/// </summary>
public enum AuthorizationSchemes
{
    /// <summary>
    /// Bearer token authorization scheme commonly used in web APIs and OAuth 2.0 authentication.
    /// </summary>
    Bearer,

    /// <summary>
    /// Digest authorization scheme, an older HTTP authentication method that uses a challenge-response mechanism for authentication.
    /// </summary>
    Digest,

    /// <summary>
    /// Basic authorization scheme, an HTTP authentication method where credentials (username and password) are sent in the request headers as base64-encoded.
    /// </summary>
    Basic
}