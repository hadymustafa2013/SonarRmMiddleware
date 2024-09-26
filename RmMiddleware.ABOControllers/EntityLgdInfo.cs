namespace RmMiddleware.ABOControllers;

public abstract class EntityLgdInfo
{
    public string? LgdGrade { get; set; }
    public string? LgdScore { get; set; }
    public string? BorrowerGrade { get; set; }
    public string? BorrowerOverrideGraderGrade { get; set; }
    public double? BorrowerPd { get; set; }
    public double? OverridePd { get; set; }
}