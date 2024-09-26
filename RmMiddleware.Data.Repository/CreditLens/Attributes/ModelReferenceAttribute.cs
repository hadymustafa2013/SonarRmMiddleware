namespace Data.Repository.CreditLens.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ModelReferenceAttribute(string otherModelId,
    string otherModelKey, string relationship,string? oneToOneRelationshipType = null) : Attribute
{
    public readonly string? OtherModelId = otherModelId;
    public readonly string? OtherModelKey = otherModelKey;
    public readonly string? Relationship = relationship;
    public readonly string? OneToOneRelationType = oneToOneRelationshipType;
}