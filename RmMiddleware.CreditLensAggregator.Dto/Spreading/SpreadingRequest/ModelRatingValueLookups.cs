using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest.ModelRatingValueLookup;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest;

public class ModelRatingValueLookups
{
    public string? Id { get; set; }
    public List<RatingValueLookup>? RatingValueLookups { get; set; }
}