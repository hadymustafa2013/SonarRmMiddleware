namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.Statements;

public class Balance
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Value { get; set; }
    public double OriginRounding { get; set; }
    public double OriginValue { get; set; }
}