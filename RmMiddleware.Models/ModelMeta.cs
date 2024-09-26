namespace RmMiddleware.Models;

public class ModelMeta
{
    public Attribute[]? Attribute { get; set; }
    public bool AttributeSpecified { get; set; }
    public Modelref[]? ModelRef { get; set; }
    public bool ModelRefSpecified { get; set; }
    public Behavior[]? Behavior { get; set; }
    public bool BehaviorSpecified { get; set; }
    public Authattribute[]? AuthAttribute { get; set; }
    public bool AuthAttributeSpecified { get; set; }
    public bool AuthContextSpecified { get; set; }
    public Restrictedaccess? RestrictedAccess { get; set; }
    public Exclusiveaccess? ExclusiveAccess { get; set; }
    public Anonymizeaccess? AnonymizeAccess { get; set; }
    public Audit? Audit { get; set; }
    public string? Id { get; set; }
    public string? Source { get; set; }
    public bool VersionEnable { get; set; }
    public bool Hierarchy { get; set; }
    public object? HierarchyStruct { get; set; }
    public string? DataProvider { get; set; }
    public object? ModelType { get; set; }
    public object? HierarchyType { get; set; }
    public object? HierarchySource { get; set; }
    public string? ModelCategory { get; set; }
    public string? ResourceModuleId { get; set; }
    public bool NotesEnabled { get; set; }
    public object? Extends { get; set; }
    public bool Plural { get; set; }
    public bool Root { get; set; }
    public bool Embedded { get; set; }
    public bool IsLinkModel { get; set; }
    public bool Shared { get; set; }
    public string? CreateUniqueRule { get; set; }
    public string? UpdateUniqueRule { get; set; }
    public bool AllowRootPolicy { get; set; }
    public bool AllowAccessControlWithoutAttribs { get; set; }
    public bool StandardConfig { get; set; }
    public bool Dimension { get; set; }
    public bool Fact { get; set; }
    public bool AutoPublish { get; set; }
    public object? ReferenceModule { get; set; }
    public bool IncludeInDataGridCache { get; set; }
    public bool ExcludeFromReportData { get; set; }
    public bool ExcludeHierarchyFromReportData { get; set; }
    public object? ObsoleteReason { get; set; }
    public bool DisableModelEvents { get; set; }
    public object[]? CalculatedAttributes { get; set; }
}

public class Restrictedaccess
{
    public string? IsRestrictedAttribute { get; set; }
    public string? RestrictedUsersAttribute { get; set; }
    public bool? Deleted { get; set; }
}

public class Exclusiveaccess
{
    public string? IsExclusiveAttribute { get; set; }
    public string? ExclusiveUserAttribute { get; set; }
    public bool? Deleted { get; set; }
}

public class Anonymizeaccess
{
    public string? AnonymizeStatusAttribute { get; set; }
    public bool? Deleted { get; set; }
}

public class Audit
{
    public string? UserId { get; set; }
    public string? Time { get; set; }
}

public class Modelref
{
    public Refattribute[]? RefAttribute { get; set; }
    public bool RefAttributeSpecified { get; set; }
    public string? Id { get; set; }
    public string? Relation { get; set; }
    public bool Embedded { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
    public string? LinkModel { get; set; }
    public string? SrcModelId { get; set; }
    public string? ModelId { get; set; }
    public bool SystemModel { get; set; }
    public string? ResidingModule { get; set; }
    public bool TreatAsOneToOne { get; set; }
    public string? DefaultDesignator { get; set; }
    public string? OneToOneRelationType { get; set; }
    public bool? ReadOnly { get; set; }
    public bool? ExcludeFromReport { get; set; }
}

public class Refattribute
{
    public string? Id { get; set; }
    public string? SourceId { get; set; }
    public bool Deleted { get; set; }
    public bool Shared { get; set; }
}

public class Behavior
{
    public string? Signature { get; set; }
    public bool Plural { get; set; }
    public string? Type { get; set; }
    public object? RuleName { get; set; }
    public bool Shared { get; set; }
    public bool Deleted { get; set; }
    public object? Text { get; set; }
}

public class Authattribute
{
    public string? RefModelAttribute { get; set; }
    public bool? Array { get; set; }
    public bool? LeafOnly { get; set; }
    public bool? Deleted { get; set; }
}