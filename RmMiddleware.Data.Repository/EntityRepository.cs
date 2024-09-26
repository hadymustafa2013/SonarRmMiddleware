using Data.Repository.CreditLens;
using Data.Repository.POCO;

namespace Data.Repository;

public static class EntityRepository

{
    public static async Task<List<Entity>> Search(Dictionary<string, object> search)
    {
        var helpers = new TypedWrapper<Entity>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
        
        return await helpers.SearchViewModel(search);
    }
    
    public static async Task<string?> LookupCommercialRegistrationCodeGivenEntityIdAndVerifyConsent(int entityId)
    {
        var search = new Dictionary<string, object>
        {
            { "EntityId", entityId }
        };
        var entities = await EntityRepository.Search(search);

        if (entities.Count == 0) throw new Exception("Entity Not Found.");
        
        if (!entities[0].Consent == true || !entities[0].Consent.HasValue) throw new Exception("No consent given. Update Entity to provide consent.");

        var legalEntity = entities[0].LegalEntity;
        return legalEntity;
    }
}