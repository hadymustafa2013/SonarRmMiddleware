using Data.Repository.CreditLens.Models;
using Newtonsoft.Json;
using RmMiddleware.Helpers;

namespace Data.Repository.CreditLens;

public class AuthoriseCreditLens
{
    public static Dictionary<string, string?> AuthorizeWithCreditLensAndCreateBearerTokenHeadersForApiRequests(
        Uri baseUri, string? userName, string? password)
    {
        var uri = new Uri(baseUri, "/api/security/authenticate");

        var authenticationRequest = new AuthenticationRequest
        {
            UserName = userName,
            Password = password
        };

        var authenticationRequestString = JsonConvert.SerializeObject(authenticationRequest);
        var response = HttpClientHelper
            .PostStringBodyReturnJObject(uri, authenticationRequestString, null)
            .Result;

        if (response?["status"] == null) throw new Exception("Not Authorised.");

        return new Dictionary<string, string?> { { "Authorization", $"Bearer {response["payLoad"]?["token"]}" } };
    }
}