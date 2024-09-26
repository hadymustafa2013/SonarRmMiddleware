using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

[Behaviour("void ImportBayanStatementList(Model m)")]
public class BayanStatementRequest
{
    [PrimaryKey("Assigned")]
    public long Id { get; set; }
    public string? Payload { get; set; }
    [ModelReference("Entity","EntityId","OneToOne","Child")]
    public long? EntityId { get; set; }
    public string? ErrorStack { get; set; }
    public DateTime? RequestDate { get; set; }
    public bool Success { get; set; }
    public long ResponseTime { get; set; }
    public string? RequestUser { get; set; }
}