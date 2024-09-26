using System.Reflection;
using Data.Repository.CreditLens.Attributes;
using Data.Repository.CreditLens.Models;
using Newtonsoft.Json;
using RmMiddleware.Helpers;

namespace Data.Repository.CreditLens;

public class Cryptography<T>
{
    private readonly Uri _baseUri;

    // ReSharper disable once MemberCanBePrivate.Global
    public readonly Dictionary<string, string?>? Headers;

    public Cryptography(Uri baseUri, string? userName, string? password)
    {
        _baseUri = baseUri;
        Headers = AuthoriseCreditLens.AuthorizeWithCreditLensAndCreateBearerTokenHeadersForApiRequests
            (_baseUri, userName, password);
    }

    public Cryptography(Uri baseUri, Dictionary<string, string?>? headers)
    {
        _baseUri = baseUri;
        Headers = headers;
    }
    
    public async Task<T> DecryptPropertiesWithEncryptAttribute(T t)
    {
        var props = t?.GetType().GetProperties();
        if (props == null) return t;

        foreach (var prop in props)
        {
            foreach (var attribute in prop.GetCustomAttributes())
            {
                if (attribute.GetType() != typeof(EncryptAttribute)) continue;
                var value = await Decrypt(prop.GetValue(t)?.ToString());
                prop.SetValue(t, value);
            }
        }

        return t;
    }

    private async Task<string?> Decrypt(string? value)
    {
        var data = new EncryptionDecryptionData
        {
            Data = value
        };

        var jObject =
            await HttpClientHelper.PostStringBodyReturnJObject(new Uri(_baseUri, "/api/secure/data/decryptAll"),
                JsonConvert.SerializeObject(data), Headers);
        return jObject?.ToObject<EncryptionDecryptionData>()?.Data;
    }
}