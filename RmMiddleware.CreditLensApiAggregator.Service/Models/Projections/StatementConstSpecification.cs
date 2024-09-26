namespace RmMiddleware.CreditLensApiAggregator.Service.Models.Projections;

public class StatementConstSpecification
{
    public int Id { get; set; }
    public string? Label { get; set; }
    public string? ProjDefault { get; set; }
    public string? HistDefault { get; set; }
    public string? RollingDefault { get; set; }
    public int OrderIndex { get; set; }
}