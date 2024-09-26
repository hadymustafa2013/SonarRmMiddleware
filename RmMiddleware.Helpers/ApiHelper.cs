using System.Text;
using Newtonsoft.Json;
using RmMiddleware.Models.DTO;

namespace RmMiddleware.Helpers;

public static class ApiHelper
{
    public static async Task<CreditLensSearchResponse?> ExecuteGetAsync(HttpClient client, string url)
    {
        var searchResponse = await client.GetAsync(url);
        var searchResponseString = await searchResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CreditLensSearchResponse>(searchResponseString);
    }

    public static async Task<CreditLensSearchResponse?> ExecutePostAsync(HttpClient client, string url,
        Dictionary<string, object>? content)
    {
        var searchResponse = await client.PostAsync(url, CreateStringContentFromDictionary(content));
        var searchResponseString = await searchResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CreditLensSearchResponse>(searchResponseString);
    }

    private static StringContent? CreateStringContentFromDictionary(Dictionary<string, object>? dictionary)
    {
        if (dictionary == null) return null;

        var entitySearchJson = JsonConvert.SerializeObject(dictionary);
        return new StringContent(entitySearchJson, Encoding.UTF8, "application/json");
    }
}