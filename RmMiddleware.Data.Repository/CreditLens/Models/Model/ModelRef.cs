namespace Data.Repository.CreditLens.Models.Model;

public class ModelRef
{
    public bool? Embedded { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<RefAttribute>? RefAttribute { get; set; }
    public string? ModelId { get; set; }
    public string? Relation { get; set; }
    public bool? TreatAsOneToOne { get; set; }
    public string? DefaultDesignator { get; set; }
    public string? Description { get; set; }
    public string? Id { get; set; }
    public string? OneToOneRelationType { get; set; }
}