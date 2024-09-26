using Data.Repository.CreditLens;
using Data.Repository.POCO;

namespace Data.Repository;

public static class BayanStatementRepository

{
    public static async Task<List<BayanStatement>> Search(Dictionary<string, object> search)
    {
        var helpers = new TypedWrapper<BayanStatement>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        
        return await helpers.SearchViewModel(search);
    }
    
    public static async Task InsertIfNotExists(BayanStatement value, Dictionary<string, object> upsertSearch)
    {
        var helpers = new TypedWrapper<BayanStatement>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        var id = await helpers.ExistsSearchViewModel(upsertSearch);
        if (id == null)
        {
            await helpers.Insert(value);
        }
    }
    
    public static async Task<BayanStatement> Update(BayanStatement value)
    {
        var helpers = new TypedWrapper<BayanStatement>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        
        return await helpers.Update(value);
    }
}