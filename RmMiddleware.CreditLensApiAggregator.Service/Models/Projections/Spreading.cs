using RmMiddleware.CreditLensApiAggregator.Service.Models.Domains;
using RmMiddleware.CreditLensApiAggregator.Service.Models.Projections.Statements;

namespace RmMiddleware.CreditLensApiAggregator.Service.Models.Projections;

public class Spreading
{
    public Entity? Entity { get; set; }
    public ModelProperties? ModelProperties { get; set; }
    public List<Statement>? ProjHistStatements { get; set; }
    public List<Statement>? Statements { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<Accounts>? Accounts { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<StatementConstSpecification>? StmtConsts { get; set; }
}