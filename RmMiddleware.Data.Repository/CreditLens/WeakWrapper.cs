using Newtonsoft.Json.Linq;
using RmMiddleware.Helpers;
using Attribute = Data.Repository.CreditLens.Models.Model.Attribute;

namespace Data.Repository.CreditLens;

public class WeakWrapper
{
    private readonly Uri _baseUri;

    // ReSharper disable once MemberCanBePrivate.Global
    public readonly Dictionary<string, string?>? Headers;

    public WeakWrapper(Uri baseUri, string? userName, string? password)
    {
        _baseUri = baseUri;
        Headers = AuthoriseCreditLens.AuthorizeWithCreditLensAndCreateBearerTokenHeadersForApiRequests
            (_baseUri, userName, password);
    }

    public WeakWrapper(Uri baseUri, Dictionary<string, string?>? headers)
    {
        _baseUri = baseUri;
        Headers = headers;
    }

    public async Task AddAttributeToModel(string model, Attribute attribute)
    {
        var jObject = await HttpClientHelper
            .GetReturnJObject(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/models/{model}"), Headers);

        AppendAttributeToCurrentModelAttributeList(attribute, jObject);

        await HttpClientHelper
            .PutStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/models/{model}"),
                jObject?.ToString(), Headers);
    }

    private static void AppendAttributeToCurrentModelAttributeList(Attribute attribute, JObject? jObject)
    {
        if (jObject == null) return;
        
        var attributes = jObject["attribute"]?.ToObject<JArray>();
        attributes?.Add(JToken.FromObject(attribute));
        jObject["attribute"] = attributes;
    }

    public async Task AddAttributeToView(string view,
        int group,
        Data.Repository.CreditLens.Models.View.Attribute attribute,
        int? subGroup = null)
    {
        var jObject = await HttpClientHelper
            .GetReturnJObject(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/viewModels/{view}"), Headers);

        var searchModel = ReturnIfSearchModel(jObject);
        var attributes = GetAttributesFromView(group, subGroup, searchModel, jObject);
        var version = GetVersionAndIncrement(jObject);
        
        AppendAttributeToCurrentViewAttributeList(group, attribute, subGroup, jObject, attributes, searchModel, version);

        await HttpClientHelper
            .PutStringBodyOnly(
                new Uri(_baseUri, $"/api/meta/v1/modules/Tenant/viewModels/{view}"),
                jObject?.ToString(), Headers);
        
        await HttpClientHelper.PutStringBodyOnly(new Uri(_baseUri,
                $"/api/meta/v1/modules/Tenant/viewModels/{view}/versions/{version}/activate"), null, Headers
        );
    }

    private static void AppendAttributeToCurrentViewAttributeList(int group, Models.View.Attribute attribute, int? subGroup,
        JObject? jObject, JArray? attributes, bool searchModel, int version)
    {
        if (jObject == null) return;
        
        attribute.Order = attributes?.Count + 1;
        attributes?.Add(JToken.FromObject(attribute));
            
#pragma warning disable CS8602 // Dereference of a possibly null reference.

        if (searchModel)
        {
            jObject["viewModel"]["group"][group]["attribute"] = attributes;
        }
        else
        {
            if (subGroup.HasValue)
            {
                jObject["viewModel"]["group"][group]["group"][subGroup]["attribute"] = attributes;
            }
            else
            {
                jObject["viewModel"]["group"][group]["attribute"] = attributes;
            }
        }
            
        jObject["version"] = version;

#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    private static int GetVersionAndIncrement(JObject? jObject)
    {
        var version = int.Parse(jObject?["version"]?.ToString() ?? string.Empty) + 1;
        return version;
    }

    private static bool ReturnIfSearchModel(JObject? jObject)
    {
        var searchModel = bool.Parse(jObject?["viewModel"]?["isSearch"]?.ToString() ?? string.Empty);
        return searchModel;
    }

    private static JArray? GetAttributesFromView(int group, int? subGroup, bool searchModel, JObject? jObject)
    {
        JArray? attributes;
        if (searchModel)
        {
            attributes = jObject?["viewModel"]?["group"]?[group]?["attribute"]?.ToObject<JArray>();
        }
        else
        {
            attributes = subGroup.HasValue ? jObject?["viewModel"]?["group"]?[group]?["group"]
                    ?[subGroup]?["attribute"]?.ToObject<JArray>() 
                : jObject?["viewModel"]?["group"]?[group]?["attribute"]?.ToObject<JArray>();
        }

        return attributes;
    }

    public async Task<JObject?> PostReturnJObject(string endpoint, string? body)
    {
        return await HttpClientHelper
            .PostStringBodyReturnJObject(new Uri(_baseUri, endpoint), body, Headers);
    }
    
    public async Task<JArray?> PostReturnJArray(string endpoint, string? body)
    {
        return await HttpClientHelper
            .PostStringBodyReturnJArray(new Uri(_baseUri, endpoint), body, Headers);
    }

    public async Task PostStringBodyOnly(string endpoint, string? body)
    {
        await HttpClientHelper
            .PostStringBodyOnly(new Uri(_baseUri, endpoint), body, Headers);
    }

    public async Task<JObject> PostFileOnly(string endpoint, string fileName, Stream stream)
    {
        return await HttpClientHelper
            .PostFileStream(new Uri(_baseUri, endpoint), stream, fileName, Headers);
    }

    public async Task PutStringBodyOnly(string endpoint, string? body)
    {
        await HttpClientHelper
            .PutStringBodyOnly(new Uri(_baseUri, endpoint), body, Headers);
    }
}