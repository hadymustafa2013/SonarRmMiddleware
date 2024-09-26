namespace Data.Repository.POCO;

public class Projection
{
    public int Id { get; set; }
    public string? AssumpCalcMethod { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int DefIntCalcType { get; set; }
    public double DeficitSplitPercent { get; set; }
    public double DetailDataEntry { get; set; }
    public double DisplayRounding { get; set; }
    public int EnableComponent { get; set; }
    public string? EntityId { get; set; }
    public string? FinancialId { get; set; }
    public string? FinancialTemplate { get; set; }
    public double GrossSales { get; set; }
    public double GrossSales2 { get; set; }
    public double GrossSales3 { get; set; }
    public bool IsGrossSalesOverwritten { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? Name { get; set; }
    public int NumberOfProjStmts { get; set; }
    public bool PreviewResults { get; set; }
    public int ProformaCopyId { get; set; }
    public int ProformaHistId { get; set; }
    public int ProjectionCalculationMethod { get; set; }
    public int ProjectionType { get; set; }
    public int ScenarioType { get; set; }
    public double SurplusSplitPrecent { get; set; }
    public int ViewCount { get; set; }
}