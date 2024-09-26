namespace RmMiddleware.Models;

public class ViewModelMeta
{
    public string? ViewModelId { get; set; }
    public VmGroup[]? Groups { get; set; }
    public Toolbarbutton[]? ToolbarButtons { get; set; }
    public string? PrimaryModel { get; set; }
    public int? MaxSearchResultCount { get; set; }
    public bool? IsSearch { get; set; }
    public int? PageSize { get; set; }
    public bool? IsInfiniteScroll { get; set; }
    public bool? Hierarchy { get; set; }
    public bool? NotesEnabled { get; set; }
    public string? Type { get; set; }
    public int ColSpan { get; set; }
    public object? ModuleId { get; set; }
    public bool? Shared { get; set; }
    public string? ResourceModuleId { get; set; }
    public bool AllowDraft { get; set; }
    public bool DisableCreate { get; set; }
    public bool UseBehavior { get; set; }
    public bool DesignModeDisabled { get; set; }
}

public class VmGroup
{
    public VmAttribute[]? Attributes { get; set; }
    public Viewmodelref[]? ViewModelRefs { get; set; }
    public VmGroup[]? Groups { get; set; }
    public string? Id { get; set; }
    public object? ResourceId { get; set; }
    public string? Type { get; set; }
    public int ColSpanOverride { get; set; }
    public int Order { get; set; }
    public string? Action { get; set; }
    public string? ActionRule { get; set; }
    public object? ActionConfigSetting { get; set; }
    public object? Expression { get; set; }
    public bool? Shared { get; set; }
    public bool? Deleted { get; set; }
    public bool? DesignModeDisabled { get; set; }
}

public class VmAttribute
{
    public string? AttributeId { get; set; }
    public string? ModelId { get; set; }
    public string? ModelFieldId { get; set; }
    public object? CellIcon { get; set; }
    public object? CellTooltip { get; set; }
    public bool Encrypt { get; set; }
    public bool Calculated { get; set; }
    public string? Source { get; set; }
    public object? AttributeExpression { get; set; }
    public object? AttributeRule { get; set; }
    public string? ControlType { get; set; }
    public string? DataType { get; set; }
    public object[]? RunRule { get; set; }
    public int MaxLength { get; set; }
    public object? ResourceId { get; set; }
    public object? Format { get; set; }
    public int Decimals { get; set; }
    public bool? Readonly { get; set; }
    public bool Array { get; set; }
    public string? RefId { get; set; }
    public object? LevelId { get; set; }
    public bool FilteredLevel { get; set; }
    public bool IsHierarchy { get; set; }
    public bool LeafOnly { get; set; }
    public bool StoreKeyValue { get; set; }
    public bool DisplayAsKey { get; set; }
    public bool MappingType { get; set; }
    public object? Width { get; set; }
    public bool Filter { get; set; }
    public bool Editable { get; set; }
    public bool Sortable { get; set; }
    public bool Resizeable { get; set; }
    public bool IsAuthAttribute { get; set; }
    public bool Hidden { get; set; }
    public object? Expression { get; set; }
    public string? GroupId { get; set; }
    public object? GroupResourceId { get; set; }
    public bool PrimaryField { get; set; }
    public object? RefParams { get; set; }
    public object? AuthParams { get; set; }
    public object? VmRefId { get; set; }
    public object? ParentAttributeId { get; set; }
    public bool DisplayField { get; set; }
    public bool IdField { get; set; }
    public bool MultiSelectField { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
    public object? Action { get; set; }
    public string? ActionRule { get; set; }
    public string? ActionConfigSetting { get; set; }
    public object? DefaultVal { get; set; }
    public object? ViewFieldId { get; set; }
    public object? ViewLabelFieldId { get; set; }
    public bool NotesEnabled { get; set; }
    public int Order { get; set; }
    public bool ChildModelIdFilter { get; set; }
    public bool VmRefParam { get; set; }
    public object? SortBy { get; set; }
    public string? ResourceModuleId { get; set; }
    public object[]? DependsOnAttributes { get; set; }
    public bool SetDefaultOnly { get; set; }
    public object? LookupDependency { get; set; }
    public bool AllowManualEdit { get; set; }
    public bool ClarifyTextEnabled { get; set; }
    public string? LinkType { get; set; }
    public object? TargetType { get; set; }
    public object? LinkCondition { get; set; }
    public object? TargetViewModelId { get; set; }
    public object? TargetGroupId { get; set; }
    public object? TargetRouteId { get; set; }
    public object? ModalTitle { get; set; }
    public object? TargetAlternateModelId { get; set; }
    public object? TargetAlternateAttributeId { get; set; }
    public string? HorizontalAlign { get; set; }
}

public class Viewmodelref
{
    public string? Id { get; set; }
    public string? GroupId { get; set; }
    public bool Lazy { get; set; }
    public bool Embedded { get; set; }
    public int Order { get; set; }
    public string? Relation { get; set; }
    public bool IsTreatAsOneToOne { get; set; }
    public string? Type { get; set; }
    public string? Side { get; set; }
    public bool Hidden { get; set; }
    public bool RefreshItems { get; set; }
    public bool CanAdd { get; set; }
    public bool CanEdit { get; set; }
    public bool? CanDelete { get; set; }
    public bool? CanDuplicate { get; set; }
    public bool? CellEdit { get; set; }
    public object? Action { get; set; }
    public string? ActionRule { get; set; }
    public string? ActionConfigSetting { get; set; }
    public object? Expression { get; set; }
    public object? Description { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
    public string? ModelId { get; set; }
    public string? LinkType { get; set; }
    public object? TargetType { get; set; }
    public object? TargetViewModelId { get; set; }
    public object? TargetGroupId { get; set; }
    public object? TargetRouteId { get; set; }
}

public class Reqval
{
    public bool Value { get; set; }
    public object? Message { get; set; }
    public object? ConditionGroupId { get; set; }
    public bool Deleted { get; set; }
    public bool? Shared { get; set; }
}

public class Patternval
{
    public string? Regex { get; set; }
    public object? Message { get; set; }
    public bool? Deleted { get; set; }
    public bool? Shared { get; set; }
}

public class Ruleval
{
    public string? RuleName { get; set; }
    public object? ConditionGroupId { get; set; }
}

public class Refparam
{
    public string? Param { get; set; }
    public string? FilterId { get; set; }
    public bool SetDefaultOnly { get; set; }
    public int Level { get; set; }
    public object? ParentAttributeId { get; set; }
}

public class Toolbarbutton
{
    public string? Id { get; set; }
    public string? Icon { get; set; }
    public object? Tooltip { get; set; }
    public bool SaveOnClick { get; set; }
    public string? Type { get; set; }
    public string? Name { get; set; }
    public object? Action { get; set; }
    public object? ActionRule { get; set; }
    public int? Order { get; set; }
    public bool? Deleted { get; set; }
    public bool? Shared { get; set; }
    public bool? Plural { get; set; }
}