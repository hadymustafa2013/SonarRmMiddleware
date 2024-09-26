namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest.Search;

public class Search
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<EntitySearchView>? Entity { get; set; }
    public Projection? Projection { get; set; }
}