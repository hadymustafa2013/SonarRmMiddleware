namespace RmMiddleware.CreditLensApiAggregator.Service.Models.Projections.Statements;

public class Balance
{
    public int Id { get; set; }
    public string? Label { get; set; }
    public double Value { get; set; }
    public double OriginRounding { get; set; }
    public double OriginValue { get; set; }
}