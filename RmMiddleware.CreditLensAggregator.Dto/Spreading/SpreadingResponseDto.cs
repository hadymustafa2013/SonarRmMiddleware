using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading;

public class SpreadingResponseDto
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Entity>? Entities { get; set; } = [];
}