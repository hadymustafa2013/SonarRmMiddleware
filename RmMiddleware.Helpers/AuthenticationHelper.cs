using System.Text;
using Newtonsoft.Json;
using RmMiddleware.Models;

namespace RmMiddleware.Helpers;

public static class AuthenticationHelper
{
    private const string AuthenticationUrl = "api/security/authenticate";
    private const string ClientName = "creditlens";

    public static async Task<HttpResponseMessage> AuthenticateAsync(IHttpClientFactory httpClientFactory,
        ClCredentials credentials)
    {
        var client = httpClientFactory.CreateClient(ClientName);
        var jsonRequest = JsonConvert.SerializeObject(credentials);
        var clResponse = await client.PostAsync(AuthenticationUrl,
            new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
        return clResponse;
    }
}