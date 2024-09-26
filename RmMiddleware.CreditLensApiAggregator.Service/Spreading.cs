using Data.Repository.CreditLens;
using Data.Repository.POCO;
using log4net;
using Newtonsoft.Json;
using Entity = RmMiddleware.CreditLensApiAggregator.Service.Models.Domains.Entity;

namespace RmMiddleware.CreditLensApiAggregator.Service;

public class Spreading(WeakWrapper weakWrapper)
{
    public async Task<List<Models.Projections.Spreading>?> GetProjectionSpreading(Dictionary<string, object>? entitySearchKeyValuePairs,
        List<int>? requiredMacroIds, List<int?>? projectionTypesIds, List<int?>? scenarioTypeIds,ILog log)
    {
        var value = new List<Models.Projections.Spreading>();
        
        List<Data.Repository.POCO.Entity> entityList = [];
        if (entitySearchKeyValuePairs is { Count: > 0 })
        {
            log.Info($"There are {entitySearchKeyValuePairs.Count} entity key value pairs. " +
                     $"Creating a typed wrapper for Entity domain. " +
                     $"Using existing header authentication with {weakWrapper.Headers?.Count} values.");
            
            var typedWrapper = new TypedWrapper<Data.Repository.POCO.Entity>(
                new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
                weakWrapper.Headers);
            
            log.Info($"Typed entity wrapper is now authenticated. Will now query the search view.");

            entityList = await typedWrapper.SearchViewModel(entitySearchKeyValuePairs);
            
            log.Info($"Search view has returned {entityList.Count} entities.");
        }

        foreach (var entity in entityList.Take(10))
        {
            log.Info($"Starting to process Entity Id {entity.EntityId} " +
                     $"from Search View results.  Getting most recent projection for projection types " +
                     $"{JsonConvert.SerializeObject(projectionTypesIds)} and scenario types " +
                     $"{JsonConvert.SerializeObject(scenarioTypeIds)}.");
            
            var projection =
                await GetLatestApprovedProjection(entity.EntityId, projectionTypesIds, scenarioTypeIds,log);
            
            if (projection == null) continue;

            log.Info($"Projection id is {projection.Id}.  Fetching the projection spreading.");
            
            var jObject =
                await weakWrapper.PostReturnJObject($"/api/financials/spreading/load/{entity.EntityId}/{projection.Id}",
                    "");
            
            if (log.IsDebugEnabled) log.Debug($"Spreading returned {jObject}.  Will proceed to deserialize.");
            
            var projectionSpreading = jObject?.ToObject<Models.Projections.Spreading>();

            log.Info("Deserialized spreading response.  " +
                     "Will proceed to enrich the model with mapped values. " +
                     "Will Map Statement Balance Account Labels.");
            
            MapStatementBalanceAccountLabels(projectionSpreading);
            
            log.Info("Will map Map Statement Constant Labels.");
            
            MapStatementConstantLabels(projectionSpreading);
            
            log.Info("Will map Map Projected Statement Balance Account Labels.");
            
            MapProjectedStatementBalanceAccountLabels(projectionSpreading);
            
            log.Info("Will map Map Projected Statement Constant Labels.");
            
            MapProjectedStatementConstantLabels(projectionSpreading);

            if (entity.EntityId != null)
            {
                log.Info("Will map Map Macros.");
                
                await MapMacros(entity.EntityId, projection.Id, projectionSpreading, requiredMacroIds);

                log.Info("Will map Map Entity.");
                
                MapEntity(entity, projection, projectionSpreading);
            }
            else
            {
                log.Info("Can't map Map Macros as the Entity Id is null.");
            }

            if (projectionSpreading == null) continue;
            
            value.Add(projectionSpreading);
                
            if (log.IsDebugEnabled) log.Debug($"Added {JsonConvert.SerializeObject(projectionSpreading)}");
        }

        return value;
    }

    private static void MapEntity(Data.Repository.POCO.Entity entity, Projection projection, Models.Projections.Spreading? projectionSpreading)
    {
        var entityView = new Entity
        {
            Id = entity.EntityId,
            ProjectionId = projection.Id,
            ProjectionName = projection.Name,
            ProjectionModifiedDate = projection.ModifiedDate
        };

        if (projectionSpreading != null) projectionSpreading.Entity = entityView;
    }

    private async Task MapMacros(string entityId, int projectionId, Models.Projections.Spreading? projectionSpreading,
        IEnumerable<int>? requiredMacroIds)
    {
        var serviceMacroStatementValues = new MacroStatementValues(weakWrapper);
        if (requiredMacroIds != null)
        {
            foreach (var requiredMacroId in requiredMacroIds)
            {
                var macroStatementValues =
                    await serviceMacroStatementValues.GetMacroStatementValues(entityId, projectionId, requiredMacroId);

                if (macroStatementValues == null) continue;

                foreach (var macroStatementValue in macroStatementValues)
                {
                    var statementProjection =
                        projectionSpreading?.ProjHistStatements?.FirstOrDefault(f =>
                            f.Id == macroStatementValue.StmtId);

                    macroStatementValue.Id = requiredMacroId;
                    if (statementProjection != null)
                    {
                        statementProjection.MacroStatementValues.Add(macroStatementValue);
                        continue;
                    }

                    var statementHistoric =
                        projectionSpreading?.Statements?.FirstOrDefault(f => f.Id == macroStatementValue.StmtId);

                    statementHistoric?.MacroStatementValues.Add(macroStatementValue);
                }
            }
        }
    }

    private static void MapProjectedStatementBalanceAccountLabels(Models.Projections.Spreading? projectionSpreading)
    {
        if (projectionSpreading?.ProjHistStatements == null) return;

        foreach (var balance in projectionSpreading.ProjHistStatements.SelectMany(statement => statement.Balances))
        {
            balance.Label = projectionSpreading?.Accounts?.FirstOrDefault(f => f.Id == balance.Id)?.Label;
        }
    }

    private static void MapProjectedStatementConstantLabels(Models.Projections.Spreading? projectionSpreading)
    {
        if (projectionSpreading?.ProjHistStatements == null) return;

        foreach (var statementConst in projectionSpreading.ProjHistStatements.SelectMany(statement =>
                     statement.StatementConsts))
        {
            statementConst.Label =
                projectionSpreading?.StmtConsts?.FirstOrDefault(f => f.Id == statementConst.Id)?.Label;
        }
    }

    private static void MapStatementBalanceAccountLabels(Models.Projections.Spreading? projectionSpreading)
    {
        if (projectionSpreading?.Statements == null) return;

        foreach (var balance in projectionSpreading.Statements.SelectMany(statement => statement.Balances))
        {
            balance.Label = projectionSpreading?.Accounts?.FirstOrDefault(f => f.Id == balance.Id)?.Label;
        }
    }

    private static void MapStatementConstantLabels(Models.Projections.Spreading? projectionSpreading)
    {
        if (projectionSpreading?.Statements == null) return;

        foreach (var statementConst in
                 projectionSpreading.Statements.SelectMany(statement => statement.StatementConsts))
        {
            statementConst.Label =
                projectionSpreading?.StmtConsts?.FirstOrDefault(f => f.Id == statementConst.Id)?.Label;
        }
    }

    private async Task<List<Projection?>> GetAllProjections(string? entityId,ILog log)
    {
        log.Info($"About to call the projections list api for entity Id {entityId}.");
        
        var jArray = await weakWrapper.PostReturnJArray($"/api/financials/projection/AllProjections/{entityId}", "");

        if (log.IsDebugEnabled) log.Debug($"Returned projections list {jArray}.");
        
        var value = new List<Projection?>();

        if (jArray == null) return value;

        log.Info($"Projections list is not null and will map all projections to an object to return.");
        
        value.AddRange(jArray.Select(jToken => jToken.ToObject<Projection>()));

        log.Info($"Mapped projections array.");
        
        return value;
    }

    private async Task<Projection?> GetLatestApprovedProjection(string? entityId, List<int?>? projectionTypesIds,
        List<int?>? scenarioTypeIds,ILog log)
    {
        return (await GetAllProjections(entityId,log))
            .Where(w => w != null
                        && (projectionTypesIds == null || projectionTypesIds.Contains(w.ProjectionType)) 
                        && (scenarioTypeIds == null || scenarioTypeIds.Contains(w.ScenarioType)))
            .MaxBy(o => o?.Id);
        
    }
}