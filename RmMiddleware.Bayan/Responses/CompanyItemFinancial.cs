namespace RmMiddleware.Bayan.Responses;

public class CompanyItemFinancial
{
    public int FilingId { get; set; }
    public DateTime Annuity { get; set; }
    public int NatureCode { get; set; }
    public string? NatureDescription { get; set; }
    public bool IsResubmission { get; set; }
    public string? ResubmissionComments { get; set; }
}