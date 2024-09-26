namespace Data.Repository.CreditLens.Models.Model;

public class Request
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Attribute>? Attribute { get; set; } 
    public bool VersionEnable { get; set; }
    public bool Plural { get; set; }
    public bool ExcludeFromReportData { get; set; }
    public bool ExcludeHierarchyFromReportData { get; set; }
    public bool DisableModelEvents { get; set; }
    public string? Id { get; set; }
    public bool Root { get; set; }
    public string? ReferenceModule { get; set; }
    public string? Description { get; set; }
    public string? DataProvider { get; set; }
    public string? Source { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<ModelRef>? ModelRef { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Behavior>? Behavior { get; set; }
}