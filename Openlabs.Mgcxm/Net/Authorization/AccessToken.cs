// Copr. (c) Nexus 2023. All rights reserved.

using Newtonsoft.Json;

namespace Openlabs.Mgcxm.Net.Authorization;

public class AccessTokenConverter : JsonConverter<AccessToken>
{
    public override AccessToken ReadJson(JsonReader reader, Type objectType, AccessToken existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var authorizationSchemeValue = reader.ReadAsString();
        var plainTextValue = reader.ReadAsString();

        existingValue = new AccessToken();
        existingValue.AuthorizationScheme = Enum.Parse<AuthorizationSchemes>(authorizationSchemeValue);
        existingValue.PlainText = plainTextValue;

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, AccessToken value, JsonSerializer serializer)
    {
        var jTokenWriter = new Dictionary<string, object>
        {
            { "Type", value.AuthorizationScheme.ToString() },
            { "Token", value.PlainText }
        };

        serializer.Serialize(writer, jTokenWriter);
    }
}

[JsonConverter(typeof(AccessTokenConverter))]
public struct AccessToken
{
    public AccessToken(AuthorizationSchemes scheme, string plainText)
    {
        this._authorizationScheme = scheme;
        this._plainText = plainText;
    }
    
    public string ConstructString()
        => $"{_authorizationScheme} {_plainText}";

    public AuthorizationSchemes AuthorizationScheme { get { return _authorizationScheme; } set { _authorizationScheme = value; } }
    public string PlainText { get { return _plainText; } set { _plainText = value; } }
    
    private AuthorizationSchemes _authorizationScheme;
    private string _plainText;
}