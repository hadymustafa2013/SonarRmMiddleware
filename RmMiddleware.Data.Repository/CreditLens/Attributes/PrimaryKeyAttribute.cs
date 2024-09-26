namespace Data.Repository.CreditLens.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class PrimaryKeyAttribute(string? keyGenerationStrategy = null) : Attribute
{
    public readonly string? KeyGenerationStrategy = keyGenerationStrategy;
}