namespace RmMiddleware.Bayan.Responses;

public class CompanyInformation
{
    public string? CrNumber { get; set; }
    public string? CompanyNameArabic { get; set; }
    public string? NationalNumber { get; set; }
    public string? CrMainNumber { get; set; }
    public string? CompanyUnitTypeCode { get; set; }
    public string? CompanyUnitTypeDescription { get; set; }
    public bool FinancialStatementAvailable { get; set; }

    // ReSharper disable once CollectionNeverQueried.Global exists to support json serialisation
    public List<CompanyItemFinancial>? CompanyFinancials { get; set; }
}