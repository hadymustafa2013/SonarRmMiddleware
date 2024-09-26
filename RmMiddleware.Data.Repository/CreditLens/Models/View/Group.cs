namespace Data.Repository.CreditLens.Models.View;

public class Group
{
    public bool GroupSpecified { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Attribute>? Attribute { get; set; }
    public bool AttributeSpecified { get; set; }
    public bool ViewModelRefSpecified { get; set; }
    public bool ViewModelWidgetSpecified { get; set; }
    public string? Id { get; set; }
    public string? ResourceId { get; set; }
    public string? Type { get; set; }
    public string? Action { get; set; }
    public string? ActionRule { get; set; }
    public string? ActionConfigSetting { get; set; }
    public string? Expression { get; set; }
    public int ColSpanOverride { get; set; }
    public int Order { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
    public string? CloudBehavior { get; set; }
}