using Data.Repository.CreditLens;
using Data.Repository.POCO;

namespace Data.Repository;

public static class BayanStatementRequestRepository

{
    public static async Task<BayanStatementRequest> Upsert(BayanStatementRequest value,Dictionary<string,object> upsertSearch)
    {
        var helpers = new TypedWrapper<BayanStatementRequest>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        var id = await helpers.ExistsSearchViewModel(upsertSearch);
        if (id == null) return await helpers.Insert(value);
        value.Id = id.Value;
        return await helpers.Update(value);

    }
}