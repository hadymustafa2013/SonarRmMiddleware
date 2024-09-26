namespace Data.Repository.CreditLens.Models.ReferenceData.Model;

public class Attribute
{
    public string? Id { get; set; }
    public string? DataType { get; set; }
    public bool PrimaryField { get; set; }
    public string? KeyGenerationStrategy { get; set; }
    public int PrimaryFieldOrder { get; set; }
}