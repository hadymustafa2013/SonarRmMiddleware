namespace Data.Repository.CreditLens.Models.Model;

public class Attribute
{
    public bool IsNew { get; set; }
    public string? AttributeExpression { get; set; }
    public List<RefAttribute>? RefAttribute { get; set; }
    public bool? Unique { get; set; }
    public bool? Calculated { get; set; }
    public string? Id { get; set; }
    public string? DataType { get; set; }
    public string? RefModelId { get; set; }
    public bool? PrimaryField { get; set; }
    public bool? Transient { get; set; }
    public string? KeyGenerationStrategy { get; set; }
    public string? Description { get; set; }
    public string? ResourceLabel { get; set; }
    public bool Encrypt { get; set; }
}