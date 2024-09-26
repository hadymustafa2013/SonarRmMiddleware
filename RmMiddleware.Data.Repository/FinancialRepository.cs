using Data.Repository.CreditLens;
using Data.Repository.POCO;
using log4net;

namespace Data.Repository;

public static class FinancialRepository

{
    public static async Task<List<Financial>> Search(Dictionary<string, object> search)
    {
        var helpers = new TypedWrapper<Financial>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        
        return await helpers.SearchViewModel(search);
    }
    
    public static async Task<Financial> LookupFinancialIdGivenEntityIdAndPrimaryFlag(int entityId)
    {
        var search = new Dictionary<string, object>
        {
            { "EntityId", entityId },
            { "Primary", true }
        };
        var financials = await Search(search);

        if (financials.Count == 0) throw new Exception("Financial Not Found. Please check the Financial has been created and the template is MMAS.");
        
        return financials[0];
    }
}