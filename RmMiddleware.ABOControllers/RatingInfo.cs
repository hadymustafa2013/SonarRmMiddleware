namespace RmMiddleware.ABOControllers;

public class RatingInfo
{
    public int? LatestStatementId { get; set; }
    public DateTime? LastStatementDate { get; set; }
    public string? RatingModel { get; set; }
    public string? ModelGrade { get; set; }
    public string? OverrideGrade { get; set; }
    public double? FinalScore { get; set; }
    public double? ModelPd { get; set; }
    public double? OverridePd { get; set; }
    public string? OverrideReason { get; set; }
    public string? OverrideComment { get; set; }
    public DateTime? ApproveDate { get; set; }
    public bool IsApproved { get; set; }
}