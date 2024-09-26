using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

public class SourceTargetAccountMapping
{
    [PrimaryKey]
    public int Id { get; set; }
    public int TargetFinancialTemplateId { get; set; }
    public string? SourceItemCode { get; set; }
    public string? TargetItemDescription { get; set; }
    public string? SourceDocCode { get; set; }
    public int EvaluationPriority { get; set; }
    public double SourceCoefficient { get; set; }
    [Ignore]
    public bool DocCodeHeader { get; set; }
}