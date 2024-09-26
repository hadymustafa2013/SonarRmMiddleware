namespace Data.Repository.POCO.FinancialSpreading;

public class Balance
{
    public int Id { get; set; }
    public double OriginValue { get; set; }
    public int OriginRounding { get; set; }
    public bool IsModified { get; set; }
    public bool Uda { get; set; }
}