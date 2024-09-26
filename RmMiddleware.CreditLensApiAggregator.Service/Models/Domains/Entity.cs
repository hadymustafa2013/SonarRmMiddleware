namespace RmMiddleware.CreditLensApiAggregator.Service.Models.Domains;

public class Entity
{
    public string? Id { get; set; }
    public int? ProjectionId { get; set; }
    public string? ProjectionName { get; set; }
    public DateTime? ProjectionModifiedDate { get; set; }
    public string? Cif { get; set; }
}