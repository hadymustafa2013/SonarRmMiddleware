namespace Data.Repository.POCO;

public class Financial
{
    public int Id { get; set; }
    public bool AllowCombinedStmt { get; set; }
    public string? BatchType { get; set; }
    public int DisplayRounding { get; set; }
    public long EntityId { get; set; }
    public int HiddenAccounts { get; set; }
    public int HiddenClasses { get; set; }
    public bool Primary { get; set; }
    public int ReportRounding { get; set; }
    public DateTime RmaSubmissionDate { get; set; }
    public bool RoundBalance { get; set; }
    public bool ShowAccountsWithValue { get; set; }
    public bool ShowFinancialOnly { get; set; }
    public FinancialStatement? FinancialStatement { get; set; }
    public string? FinancialTemplate { get; set; }
}