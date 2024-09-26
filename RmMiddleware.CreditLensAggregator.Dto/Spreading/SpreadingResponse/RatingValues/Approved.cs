namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.RatingValues;

public class Approved
{
    public string? ModelId { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Factor>? Values { get; set; }
}