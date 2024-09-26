namespace Data.Repository.POCO.FinancialSpreading;

public class Context
{
    public Guid Wfid { get; set; }
    public Guid TaskId { get; set; }
    public string? State { get; set; }
    public List<ContextItem>? ContextItems { get; set; }
}