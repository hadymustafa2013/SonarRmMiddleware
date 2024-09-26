namespace Data.Repository.CreditLens.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DataTypeAttribute(string dataType) : Attribute
{
    public readonly string? DateType = dataType;
}