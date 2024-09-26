using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using StreamContent = System.Net.Http.StreamContent;

namespace RmMiddleware.Helpers;

public static class HttpClientHelper
{
    public static async Task<JObject?> GetReturnJObject(Uri uri, Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var response = await httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return JObject.Parse(await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<JObject?> PostStringBodyReturnJObject(Uri uri, string? body,
        Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var stringContent = new StringContent(body ?? string.Empty, null, "application/json");
        var response = await httpClient.PostAsync(uri, stringContent);
        response.EnsureSuccessStatusCode();
        
        return JObject.Parse(await response.Content.ReadAsStringAsync());
    }
    
    public static async Task<JArray?> PostStringBodyReturnJArray(Uri uri, string? body,
        Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var stringContent = new StringContent(body ?? string.Empty, null, "application/json");
        var response = await httpClient.PostAsync(uri, stringContent);
        response.EnsureSuccessStatusCode();
        return JArray.Parse(await response.Content.ReadAsStringAsync());
    }
    
    public static async Task PostStringBodyOnly(Uri uri, string? body,
        Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var stringContent = new StringContent(body ?? string.Empty, null, "application/json");
        var response = await httpClient.PostAsync(uri, stringContent);
        response.EnsureSuccessStatusCode();
    }
    
    public static async Task<JObject?> PutStringBodyReturnJObject(Uri uri, string? body,
        Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var stringContent = new StringContent(body ?? string.Empty, null, "application/json");
        var response = await httpClient.PutAsync(uri, stringContent);
        response.EnsureSuccessStatusCode();
        return JObject.Parse(await response.Content.ReadAsStringAsync());
    }
    
    public static async Task PutStringBodyOnly(Uri uri, string? body,
        Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var stringContent = new StringContent(body ?? string.Empty, null, "application/json");
        var response = await httpClient.PutAsync(uri, stringContent);
        response.EnsureSuccessStatusCode();
        await response.Content.ReadAsStringAsync();
    }

    public static async Task<JObject> PostFileStream(Uri uri,Stream stream, string fileName,Dictionary<string, string?>? headers)
    {
        var httpClient = CreateHttpClient(headers);
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "file", fileName);

        return JObject.Parse(await httpClient.PostAsync(uri, content).Result.Content.ReadAsStringAsync());
    }
    
    private static HttpClient CreateHttpClient(Dictionary<string, string?>? headers)
    {
        var httpProxyString = Environment.GetEnvironmentVariable("HTTP_PROXY");
        var httpProxy = httpProxyString != null && httpProxyString.Equals("True",StringComparison.OrdinalIgnoreCase);
             
        HttpClient client;
        if (httpProxy)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = 
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            client = new HttpClient(handler);
        }
        else
        {
            client = new HttpClient();
        }

        if (headers == null) return client;

        foreach (var header in headers) client.DefaultRequestHeaders.Add(header.Key, header.Value);

        return client;
    }

    public static string BasicAuthenticationHeaderString(string user, string password)
    {
        var svcCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        return "Basic " + svcCredentials;
    }
}