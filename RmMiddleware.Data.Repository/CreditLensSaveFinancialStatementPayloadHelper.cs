using Data.Repository.CreditLens;
using Data.Repository.POCO.FinancialSpreading;
using log4net;
using Newtonsoft.Json;
using Uri = System.Uri;

namespace Data.Repository;

public static class CreditLensSaveFinancialStatementPayloadHelper
{
    public static async Task Insert(CreditLensSaveFinancialStatementPayload creditLensSaveFinancialStatementPayload,ILog log,long entityId)
    {
        log.Info("Creating a weak CreditLens wrapper that will be used for spreading.  Authenticating.  Exception will present login issues.");
        
        var weakWrapper = new WeakWrapper
        (new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        log.Info("Authenticated weak wrapper against CreditLens.");
        
        var request = JsonConvert.SerializeObject(creditLensSaveFinancialStatementPayload);
        
        log.Info($"CreditLens spreading json {request}.");
        
        if (creditLensSaveFinancialStatementPayload.Context?.ContextItems != null)
        {
            log.Info($"CreditLens spreading json {request}.");

            var uri = $"api/financials/spreading/save/{entityId}/-1";
            
            log.Info($"Posting spreading to {uri}.");
            
            await weakWrapper.PostReturnJObject(uri, request);
            
            log.Info($"Concluded spreading.");
        }
        else
        {
            log.Info($"Concluded failed as there were no context items.");
        }
    }
}