using Data.Repository.CreditLens;
using Data.Repository.POCO;
using log4net;
using Newtonsoft.Json;

namespace Data.Repository;

public static class BayanSettingsRepository

{
    public static async Task<List<BayanSettings>> Search(Dictionary<string, object> search)
    {
        var helpers = new TypedWrapper<BayanSettings>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        return await helpers.SearchViewModel(search);
    }

    public static async Task<BayanSettings> LookupBayanSettingsFromFinancialTemplate(string financialTemplate,ILog log)
    {
        var search = new Dictionary<string, object>
        {
            { "FinancialTemplate", financialTemplate }
        };
        var bayanSettingsList = await Search(search);
        
        BayanSettings bayanSettings;
        if (bayanSettingsList.Count == 0)
        {
            bayanSettings = new BayanSettings
            {
                BayanHttpPassword = Environment.GetEnvironmentVariable("BAYAN_HTTP_PASSWORD") ?? "admin",
                BayanHttpUser = Environment.GetEnvironmentVariable("BAYAN_HTTP_USER") ?? "RAJHITST",
                BayanHttpEndpointJwt =
                    Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_JWT") ?? "http://localhost:5079",
                BayanHttpEndpointReport = Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_REPORT") ??
                                          "http://localhost:5079/CommercialCreditReport/1.0/",
                BayanHttpEndpointStatementList = Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_STATEMENT_LIST") ??
                                          "http://localhost:5079/CommercialCreditReport/1.0/",
                OrigRounding = int.Parse(Environment.GetEnvironmentVariable("BAYAN_ORIG_ROUNDING") ?? "0"),
                BayanDocumentUploadLocation = Environment.GetEnvironmentVariable("BAYAN_UPLOAD_LOCATION") ?? "4.1"
            };   
        }
        else
        {
            bayanSettings = bayanSettingsList[0];
        }
        
        var bayanAuthenticationEnvironmentVariable = Environment.GetEnvironmentVariable("BAYAN_AUTHENTICATION");
        
        log.Info($"BAYAN_AUTHENTICATION Environment Variable is {bayanAuthenticationEnvironmentVariable}.");
        
        bayanSettings.BayanAuthentication = bayanAuthenticationEnvironmentVariable == null || bayanAuthenticationEnvironmentVariable.Equals("True",StringComparison.OrdinalIgnoreCase);

        var bayanChannelIdHeaderEnvironmentVariable = Environment.GetEnvironmentVariable("BAYAN_CHANNEL_ID_HEADER");
        
        log.Info($"BAYAN_CHANNEL_ID_HEADER Environment Variable is {bayanChannelIdHeaderEnvironmentVariable}.");

        if (bayanChannelIdHeaderEnvironmentVariable != null)
        {
            bayanSettings.BayanChannelIdHeader = bayanChannelIdHeaderEnvironmentVariable.Equals("True",StringComparison.OrdinalIgnoreCase);   
        }

        log.Debug($"Bayan Settings are {JsonConvert.SerializeObject(bayanSettings)}.");
        
        return bayanSettings;
    }
}