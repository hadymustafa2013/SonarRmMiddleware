namespace Data.Repository.POCO.FinancialSpreading;

public class Statement
{
    public int Periods { get; set; }

    // ReSharper disable once CollectionNeverQueried.Global mainly for serialisation and as follows in class.
    public List<Balance>? Balances { get; set; }

    // ReSharper disable once CollectionNeverQueried.Global
    public List<StatementConsts>? StatementConsts { get; set; }
    public DateTime Date { get; set; }
    public int Id { get; set; }
    public bool IsDirty { get; set; }
    public bool IsNew { get; set; }
    public bool EnterBalance { get; set; }
    public bool ReconModified { get; set; }
}