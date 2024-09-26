using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest.Search.ProjectionScenarioTypes;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest.Search;

public class Projection
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ProjectionType>? ProjectionTypes { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<ScenarioType>? ScenarioTypes { get; set; }
}