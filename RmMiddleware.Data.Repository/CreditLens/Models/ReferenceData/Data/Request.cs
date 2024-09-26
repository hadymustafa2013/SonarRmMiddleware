namespace Data.Repository.CreditLens.Models.ReferenceData.Data;

public class Request
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Attribute>? Attributes { get; set; }
    public bool ModelBased { get; set; }
    public string? Id { get; set; }
    public string? Description { get; set; }
    public Query.Query? Query { get; set; }
    public string? ModelRefId { get; set; }
    public string? Type { get; set; }
}