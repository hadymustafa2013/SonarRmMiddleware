using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

[Behaviour("void ImportBayanStatementList(Model m)")]
[Behaviour("void Test(Model m)")]
public class Test9
{
    [PrimaryKey("Sequence")]
    public long Id { get; set; }
    public string? ApiResponse { get; set; }
    [ModelReference("Entity","EntityId","OneToOne")]
    public long? EntityId { get; set; }
    public string? ErrorStack { get; set; }
    public DateTime RequestDate { get; set; }
    [DataType("Date")]
    public DateTime StatementDate { get; set; }
    public bool Success { get; set; }
    public long ResponseTime { get; set; }
    public string? RequestUser { get; set; }
    [Ignore]
    public string? IgnoreMe { get; set; }
}