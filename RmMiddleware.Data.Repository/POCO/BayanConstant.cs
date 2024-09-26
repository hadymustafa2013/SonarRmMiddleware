using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

public class BayanConstant
{
    [PrimaryKey("Assigned")]
    public int Id { get; set; }
    [ModelReference("BayanSettings","Id","ManyToOne")]
    public long BayanSettingId { get; set; }
    public string? Value { get; set; }
}