namespace RmMiddleware.Bayan.Mapping.Models;

public class MappedFinancialStatement
{
    public DateTime StatementDate { get; set; }
    public string? Currency { get; set; }
    public List<AccountBalance>? AccountBalances { get; set; }
    public string? Type { get; set; }
    public bool? Consolidated { get; set; }
}