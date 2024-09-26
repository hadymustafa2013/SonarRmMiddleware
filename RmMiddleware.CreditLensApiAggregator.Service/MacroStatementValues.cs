using Data.Repository.CreditLens;
using Data.Repository.POCO.Macros;

namespace RmMiddleware.CreditLensApiAggregator.Service;

public class MacroStatementValues(WeakWrapper weakWrapper)
{
    public async Task<List<MacroStatementValue>?> GetMacroStatementValues(string entityId, int projectionId, int macroId)
    {
        var jArray = await weakWrapper.PostReturnJArray($"api/financials/calc/macros/{entityId}/{projectionId}/{macroId}", "");
        var macroStatementValues = jArray?.ToObject<List<MacroStatementValue>>();

        return macroStatementValues;
    }
}