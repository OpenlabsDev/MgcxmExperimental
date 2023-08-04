// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Net.Authorization;

public struct AccessToken
{
    public AccessToken(AuthorizationSchemes scheme, string plainText)
    {
        this._authorizationScheme = scheme;
        this._plainText = plainText;
    }
    
    public string ConstructString()
        => $"{_authorizationScheme} {_plainText}";

    public AuthorizationSchemes AuthorizationScheme => _authorizationScheme;
    public string PlainText => _plainText;
    
    private AuthorizationSchemes _authorizationScheme;
    private string _plainText;
}