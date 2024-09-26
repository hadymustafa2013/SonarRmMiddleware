namespace Data.Repository.CreditLens.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BehaviourAttribute(string signature,string type = "None", bool plural = true) : Attribute
{
    public readonly string? Type = type;
    public readonly string? Signature = signature;
    public readonly bool Plural = plural;
}