namespace RmMiddleware.Models;

public class CreditLensViewModel
{
    public string? ModuleId { get; set; } = "Tenant";
    public Viewmodel? ViewModel { get; set; }
    public string? ViewModelId { get; set; }
}

public class Viewmodel
{
    public Group[]? Group { get; set; }
    public string? Id { get; set; }
    public bool? IsSearch { get; set; }
    public string? PrimaryModel { get; set; }
    public object[]? ToolbarButton { get; set; }
    public object[]? ConditionGroup { get; set; }
}

public class Group
{
    public string? Id { get; set; }
    public bool? Nested { get; set; }
    public Attribute[]? Attribute { get; set; }
    public object[]? ViewModelRef { get; set; }
    public int? Order { get; set; }
}

public class Attribute
{
    public bool? RefAttributeSpecified { get; set; }
    public bool? AttributeSpecified { get; set; }
    public string? Id { get; set; }
    public object? DbIdentifier { get; set; }
    public string? DataType { get; set; }
    public int MaxLength { get; set; }
    public string? ResourceId { get; set; }
    public object? Rounding { get; set; }
    public string? RefId { get; set; }
    public bool IsHierarchy { get; set; }
    public string? LevelId { get; set; }
    public bool FilteredLevel { get; set; }
    public bool MappingType { get; set; }
    public bool PrimaryField { get; set; }
    public int PrimaryFieldOrder { get; set; }
    public string? KeyGenerationStrategy { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
    public bool Array { get; set; }
    public string? RefModelId { get; set; }
    public bool Transient { get; set; }
    public bool NotesEnabled { get; set; }
    public bool NotNull { get; set; }
    public object? DefaultValue { get; set; }
    public string? LinkCategory { get; set; }
    public string? LinkNode { get; set; }
    public bool LinkNodeKeys { get; set; }
    public bool Unique { get; set; }
    public bool PrimaryIndicator { get; set; }
    public bool Dimension { get; set; }
    public bool Measure { get; set; }
    public object? Tag { get; set; }
    public bool Encrypt { get; set; }
    public bool Calculated { get; set; }
    public string? Source { get; set; }
    public string? AttributeExpression { get; set; }
    public string? AttributeRule { get; set; }
    public bool? IncludeInDataGridCache { get; set; }
    public string? CloudBehavior { get; set; }
    public bool? ClarifyTextEnabled { get; set; }
    public string? ResourceModuleId { get; set; }
    public string? Tooltip { get; set; }
    public bool NewItem { get; set; }
    public string? ControlType { get; set; }
    public string? ModelId { get; set; }
    public string? ModelFieldId { get; set; }
    public int Order { get; set; }
}