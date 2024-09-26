using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest;
using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest.Search;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading;

public class SpreadingRequestDto
{
    // ReSharper disable once CollectionNeverUpdated.Global
    public Search? Search { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<FinancialTemplateMacro>? FinancialTemplateMacros { get; set; }
    public List<ModelRatingValueLookups>? ModelRatingValueLookups { get; set; }
}