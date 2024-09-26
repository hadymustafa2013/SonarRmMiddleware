using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.Statements;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse;

public class Statement
{
    public int Id { get; set; }
    public int Order { get; set; }
    public DateTime Date { get; set; }
    public int Periods { get; set; }
    public bool ApprovedRating { get; set; }
    public bool ProposedRating { get; set; }
    public ContextEnum Context { get; set; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Balance> Balances { get; set; } = [];
    // ReSharper disable once CollectionNeverQueried.Global
    public List<MacroStatementValue> FinancialTemplateMacros { get; set; } = [];
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Constant> Constants { get; set; } = [];
}