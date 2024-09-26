using Data.Repository.POCO.Macros;

namespace RmMiddleware.CreditLensApiAggregator.Service.Models.Projections.Statements;

public class Statement
{
    public int Id { get; set; }
    public int Order { get; set; }
    public DateTime Date { get; set; }
    public int Periods { get; set; }
    public int Context { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<StatementConstValue> StatementConsts { get; set; } = [];
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Balance> Balances { get; set; } = [];
    // ReSharper disable once CollectionNeverQueried.Global
    public List<MacroStatementValue> MacroStatementValues { get; set; } = [];
}