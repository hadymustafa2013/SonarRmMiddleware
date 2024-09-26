namespace Data.Repository.POCO.FinancialSpreading;

public class Payload
{
    // ReSharper disable once CollectionNeverQueried.Global mainly here for serialisation
    public List<Statement>? Statements { get; set; }
    public List<object>? Accounts { get; set; }
    public List<object>? Notes { get; set; }
    public int ProformaCopyId { get; set; }
    public int ProformaHistId { get; set; }
    public int GridType { get; set; }
}