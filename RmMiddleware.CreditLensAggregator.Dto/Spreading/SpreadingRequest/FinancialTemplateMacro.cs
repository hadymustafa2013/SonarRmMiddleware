namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingRequest;

public class FinancialTemplateMacro
{
    public int? Id { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Macro.Macro>? Macros { get; set; }
}