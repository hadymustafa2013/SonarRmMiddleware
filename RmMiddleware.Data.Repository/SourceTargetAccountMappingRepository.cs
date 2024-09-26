using Data.Repository.CreditLens;
using Data.Repository.POCO;

namespace Data.Repository;

public class SourceTargetAccountMappingRepository
{
    public static async Task<List<SourceTargetAccountMapping>> Get()
    {
        var f = new TypedWrapper<SourceTargetAccountMapping>
        (new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost")
            , Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        return await f.ReadRefData();
    }
}