using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

public class BayanStatement
{
    [PrimaryKey("Sequence")]
    public long Id { get; set; }
    public int? FilingId { get; set; }
    public long? FinancialId { get; set; }
    [DataType("Date")]
    public DateTime? StatementDate { get; set; }
    public long EntityId { get; set; }
    [ModelReference("BayanStatementRequest","Id","ManyToOne")]
    public long BayanStatementRequestId { get; set; }
    public bool Import { get; set; }
    public int SpreadCount { get; set; }
    public int TotalAccountCountFetched { get; set; }
    public int TotalAccountCountMatched { get; set; }
    public bool Fetched { get; set; }
    public DateTime? FetchedDate { get; set; }
    public string? FetchedUser { get; set; }
    public bool Success { get; set; }
    public string? ErrorStack { get; set; }
    public string? Payload { get; set; }
    public string? Pdf { get; set; }
    public string? PdfFileName { get; set; }
    public bool Resubmission { get; set; }
    public string? ResubmissionComments { get; set; }
    public string? PdfLink { get; set; }
    public int ResponseTime { get; set; }
}