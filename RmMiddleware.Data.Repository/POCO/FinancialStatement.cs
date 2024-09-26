namespace Data.Repository.POCO;

public class FinancialStatement
{
    public int Id { get; set; }
    public string? AuditMethod { get; set; }
    public int CashFlowReconcileId { get; set; }
    public string? Consolidation { get; set; }
    public DateTime Date { get; set; }
    public bool IsApproved { get; set; }
    public int Periods { get; set; }
    public string? Restated { get; set; }
    public string? Status { get; set; }
    public int StmtContext { get; set; }
    public string? Type { get; set; }
    public int Version { get; set; }
}