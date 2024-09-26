using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.RatingValues;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse;

public class RatingValue
{
    public Proposed? Proposed { get; set; }
    public Approved? Approved { get; set; }
}