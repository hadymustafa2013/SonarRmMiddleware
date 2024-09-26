using RmMiddleware.CreditLensAggregator.Dto.Spreading;
using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse;
using RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse.Statements;

namespace RmMiddleware.CreditLensAggregator.Dto.Mapping;

public class MapSpreadingResponseDto
{
    public static SpreadingResponseDto? Map(SpreadingRequestDto? spreadingRequest,
        List<CreditLensApiAggregator.Service.Models.Projections.Spreading> projections)
    {
        var spreadingResponseDto = new SpreadingResponseDto();
        foreach (var projection in projections)
        {
            if (projection.ModelProperties == null) continue;
            
            var entity = new Entity
            {
                Cif = projection.Entity?.Id,
                Id = projection.Entity?.Id,
                ProjectionId = projection.Entity?.ProjectionId,
                ProjectionName = projection.Entity?.ProjectionName,
                ProjectionModifiedDate = projection.Entity?.ProjectionModifiedDate
            };
            
            if (projection.Statements != null)
            {
                var j = projection.Statements.Count * -1 - 1;
                foreach (var statement in projection.Statements.OrderByDescending(o => o.Date))
                {
                    j += 1;
                    var statementDto = new Statement()
                    {
                        Order = j,
                        Id = statement.Id,
                        Date = statement.Date,
                        Periods = statement.Periods,
                        Context = ContextEnum.Projected
                    };

                    foreach (var constant in statement.StatementConsts.Select(statementConst => new Constant
                             {
                                 Label = statementConst.Label,
                                 Id = statementConst.Id,
                                 Value = statementConst.Value
                             }))
                    {
                        statementDto.Constants.Add(constant);
                    }

                    foreach (var macroStatementValueDto in statement.MacroStatementValues.Select(macroStatementValue =>
                                 new MacroStatementValue
                                 {
                                     Value = macroStatementValue.Value,
                                     Id = macroStatementValue.Id,
                                     Label = spreadingRequest?.FinancialTemplateMacros?
                                         .Find(f => projection.ModelProperties?.ModelId  == f.Id)
                                         ?.Macros
                                         ?.FirstOrDefault(f => f.Id == macroStatementValue.Id)
                                         ?.Label
                                 }))
                    {
                        statementDto.FinancialTemplateMacros.Add(macroStatementValueDto);
                    }

                    foreach (var balanceDto in statement.Balances.Select(balance => new Balance
                             {
                                 Id = balance.Id,
                                 Value = balance.Value,
                                 OriginRounding = balance.OriginRounding,
                                 OriginValue = balance.OriginValue,
                                 Name = balance.Label
                             }))
                    {
                        statementDto.Balances.Add(balanceDto);
                    }

                    entity.Statements?.Add(statementDto);
                }
            }

            var i = 0;
            if (projection.ProjHistStatements == null) return null;
            {
                foreach (var projHistStatement in projection.ProjHistStatements.OrderByDescending(o => o.Date))
                {
                    i += 1;
                    var statementDto = new Statement()
                    {
                        Order = i,
                        Id = projHistStatement.Id,
                        Date = projHistStatement.Date,
                        Periods = projHistStatement.Periods,
                        Context = ContextEnum.Historic
                    };

                    foreach (var constant in projHistStatement.StatementConsts.Select(statementConst => new Constant
                             {
                                 Label = statementConst.Label,
                                 Id = statementConst.Id,
                                 Value = statementConst.Value
                             }))
                    {
                        statementDto.Constants.Add(constant);
                    }
                    
                    foreach (var macroStatementValueDto in projHistStatement.MacroStatementValues.Select(
                                 macroStatementValue => new MacroStatementValue
                                 {
                                     Value = macroStatementValue.Value,
                                     Id = macroStatementValue.Id,
                                     Label = spreadingRequest?.FinancialTemplateMacros
                                         ?.Find(f => projection.ModelProperties?.ModelId  == f.Id)
                                         ?.Macros
                                         ?.FirstOrDefault(f => f.Id == macroStatementValue.Id)
                                         ?.Label
                                 }))
                    {
                        statementDto.FinancialTemplateMacros.Add(macroStatementValueDto);
                    }

                    foreach (var balanceDto in projHistStatement.Balances.Select(balance => new Balance
                             {
                                 Id = balance.Id,
                                 Value = balance.Value,
                                 OriginRounding = balance.OriginRounding,
                                 OriginValue = balance.OriginValue,
                                 Name = balance.Label
                             }))
                    {
                        statementDto.Balances.Add(balanceDto);
                    }

                    entity.Statements?.Add(statementDto);
                }
            }
            spreadingResponseDto.Entities?.Add(entity);
        }

        return spreadingResponseDto;
    }
}