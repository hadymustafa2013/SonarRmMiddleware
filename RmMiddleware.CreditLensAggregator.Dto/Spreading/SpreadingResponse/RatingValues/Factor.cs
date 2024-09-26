namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.RatingValues;

public class Factor
{
    public string? BlockId { get; set; }
    public string? Name { get; set; }
    public string? PinId { get; set; }
    public double Value { get; set; }
    public string? Label { get; set; }
}