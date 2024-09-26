using Data.Repository.CreditLens;
using Data.Repository.POCO;

namespace Data.Repository;

public static class BayanConstantRepository

{
    public static async Task<List<BayanConstant>> Search(Dictionary<string, object> search)
    {
        var helpers = new TypedWrapper<BayanConstant>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        
        return await helpers.SearchViewModel(search);
    }
    
    public static async Task<List<BayanConstant>> LookupBayanConstantsByBayanSettingId(long bayanSettingId)
    {
        var search = new Dictionary<string, object>
        {
            { "BayanSettingId", bayanSettingId }
        };
        var bayanConstantsList = await Search(search);

        return bayanConstantsList;
    }
}