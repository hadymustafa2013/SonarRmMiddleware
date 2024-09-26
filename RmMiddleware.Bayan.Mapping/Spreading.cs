using Data.Repository;
using Data.Repository.POCO;
using Data.Repository.POCO.FinancialSpreading;
using log4net;
using Newtonsoft.Json;
using RmMiddleware.Bayan.Mapping.Models;

namespace RmMiddleware.Bayan.Mapping;

public class Spreading(ILog log)
{
    public async Task CreateFinancialStatementFromBayanFilingId(int entityId, int filingId)
    {
        log.Info($"Create Financial Statement From Bayan Filing Id will " +
                 $"proceed with Entity Id {entityId} and Filing Id {filingId}. " +
                 $"Lookup Financial Id Given Entity Id And Primary Flag in the Financial Repository.");

        var financial = await FinancialRepository.LookupFinancialIdGivenEntityIdAndPrimaryFlag(entityId);

        log.Info($"Create Financial Statement From Bayan Filing ID Checking Financial Template is not null.");

        if (financial.FinancialTemplate != null)
        {
            log.Info($"Create Financial Statement returned a Filing to work with will Lookup Bayan Settings " +
                     $"From Financial Template from the Bayan Settings Repository.");

            var bayanSettings =
                await BayanSettingsRepository.LookupBayanSettingsFromFinancialTemplate(financial.FinancialTemplate,log);

            log.Info($"Bayan Settings fetched. " +
                     $"Will proceed to lookup the constants for Bayan Settings Id {bayanSettings.Id}");
            
            log.Debug($"BayanSettings: {JsonConvert.SerializeObject(bayanSettings)}");

            var bayanConstants = await BayanConstantRepository.LookupBayanConstantsByBayanSettingId(bayanSettings.Id);
            log.Info(
                $"Returned {bayanConstants.Count} statement constants.  Will write them to log one by one for trace.");

            foreach (var bayanConstant in bayanConstants)
            {
                log.Info($"Bayan Statement Constant Key {bayanConstant.Id} and Value {bayanConstant.Value}.");
            }

            log.Info("Looking up the Lookup Commercial Registration Code Given Entity Id And Verify Consent.");

            var commercialRegistrationCode =
                await EntityRepository.LookupCommercialRegistrationCodeGivenEntityIdAndVerifyConsent(entityId);

            log.Info(
                $"Looked up Commercial Registration Code Given Entity Id And Verify Consent and returned {commercialRegistrationCode}.");

            var mapping = new Mapping(log);
            var finalMappedStatement = await mapping.FetchStatementMapAndAggregate(bayanSettings, entityId,
                financial.Id, commercialRegistrationCode, filingId);

            log.Info(
                $"The financial statements have been fetched and mapped to an object that will can be spread in CreditLens. " +
                $"Will map the statements from the intermediate object (i.e., not json, not CreditLens format) to the correct payload.");

            var payload = CreatePayload();
            CreateStatementInPayload(bayanSettings, bayanConstants, finalMappedStatement, payload, log);

            log.Info(
                $"The statement payload has been created and is now ready " +
                $"for spreading with CreditLens Save Financial Statement Payload Helper.");

            await CreditLensSaveFinancialStatementPayloadHelper
                .Insert(BringContextAndPayloadTogetherForFinalRequest(entityId, financial.Id, payload), log, entityId);

            log.Info(
                $"Spread the statement.");
        }
        else
        {
            log.Info(
                $"{financial.FinancialTemplate} was not found or matched for statement.");

            throw new Exception("Financial not found.  Please check the Financial has been created and the template is MMAS.");
        }
    }

    private static CreditLensSaveFinancialStatementPayload BringContextAndPayloadTogetherForFinalRequest(int entityId,
        int financialId, Payload payload)
    {
        var context = CreateContext(entityId, financialId);
        var financialSpreading = new CreditLensSaveFinancialStatementPayload
        {
            Context = context,
            PayLoad = payload
        };
        return financialSpreading;
    }

    private static Payload CreatePayload()
    {
        var payload = new Payload
        {
            Statements = new List<Statement>(),
            ProformaCopyId = -1,
            ProformaHistId = -1,
            GridType = 0,
            Accounts = new List<object>(),
            Notes = new List<object>()
        };
        return payload;
    }

    private static void CreateStatementInPayload(BayanSettings bayanSettings, List<BayanConstant> bayanConstants,
        MappedFinancialStatement? mappedFinancialStatement, Payload payload, ILog log)
    {
        if (mappedFinancialStatement == null)
        {
            log.Info("The mapped financial statement is null.  Returning.");
            return;
        }

        log.Info("Creating statement header and constants.");

        var statement = new Statement
        {
            Id = -2147483648,
            Periods = 12,
            IsDirty = true,
            IsNew = true,
            EnterBalance = true,
            ReconModified = true,
            Balances = new List<Balance>(),
            StatementConsts = new List<StatementConsts>(),
            Date = mappedFinancialStatement.StatementDate
        };

        log.Info(
            $"Default statement header created for {mappedFinancialStatement.StatementDate}.  Will map the statement balances.");

        AddStatementBalances(bayanSettings, mappedFinancialStatement, statement, log);

        log.Info(
            $"For {mappedFinancialStatement.StatementDate}.  Has mapped the statement balances and will now map the constants.");

        AddStatementConstants(bayanConstants, statement, mappedFinancialStatement, log);

        log.Info(
            $"For {mappedFinancialStatement.StatementDate}.  Has mapped the statement constants and will add this statement to final payload.");

        payload.Statements?.Add(statement);
    }

    private static Context CreateContext(int entityId, int financialId)
    {
        var context = new Context
        {
            Wfid = new Guid(),
            TaskId = new Guid(),
            State = "CrossProcess",
            ContextItems = new List<ContextItem>
            {
                new()
                {
                    ModelId = "Entity",
                    InstanceId = entityId
                },
                new()
                {
                    ModelId = "Financial",
                    InstanceId = financialId
                }
            }
        };
        return context;
    }

    private static void AddStatementConstants(List<BayanConstant> bayanConstants,
        Statement statement, MappedFinancialStatement? mappedFinancialStatement, ILog log)
    {
        log.Info(
            "Adding the hard coded statement constants in the case that no custom statement constants have been provided.");

        CreateHardcodedStatementConstants(statement, mappedFinancialStatement);

        if (statement.StatementConsts != null)
            log.Info($"Added default statement constants {statement.StatementConsts.Count}.");

        log.Info("Merging new custom statement constants.");

        MergeHardcodedStatementConstants(bayanConstants, statement, log);

        if (statement.StatementConsts != null)
            log.Info($"Merged statement constants {statement.StatementConsts.Count}.");
    }

    private static void MergeHardcodedStatementConstants(List<BayanConstant> bayanConstants, Statement statement,
        ILog log)
    {
        log.Info($"Is about to loop around {bayanConstants.Count} custom statement constants.");

        foreach (var bayanConstant in bayanConstants)
        {
            log.Info(
                $"Statement constant {bayanConstant.Id} and value {bayanConstant.Value} will be tested against existing available constants.");

            var statementConst = statement.StatementConsts?.Find(f => f.Id == bayanConstant.Id);

            if (statementConst == null)
            {
                var unmatched = new StatementConsts
                {
                    Id = bayanConstant.Id,
                    IsModified = true,
                    Value = bayanConstant.Value
                };

                log.Info(
                    $"Statement constant {bayanConstant.Id} and value {bayanConstant.Value} unmatched.  Adding {unmatched}");

                statement.StatementConsts?.Add(unmatched);
            }
            else
            {
                log.Info(
                    $"Statement constant {bayanConstant.Id} and value {bayanConstant.Value} matched updating value.");
                statementConst.Value = bayanConstant.Value;
            }
        }
    }

    private static void CreateHardcodedStatementConstants(Statement statement,
        MappedFinancialStatement? mappedFinancialStatement)
    {
        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 1,
            IsModified = true,
            Value = "Unqualif'd"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 2,
            IsModified = true,
            Value = "Bayan"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 3,
            IsModified = true,
            Value = "Bayan"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 4,
            IsModified = true,
            Value = "Annual"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 5,
            IsModified = true,
            Value = "IFRS"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 6,
            IsModified = true,
            Value = "Draft"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 7,
            IsModified = true,
            Value = mappedFinancialStatement?.Consolidated is true ? "Consolidated" : "Unconsolidated"
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 8,
            IsModified = true,
            Value = mappedFinancialStatement?.Currency
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 9,
            IsModified = true,
            Value = ""
        });

        statement.StatementConsts?.Add(new StatementConsts
        {
            Id = 10,
            IsModified = true,
            Value = "1"
        });
    }

    private static void AddStatementBalances(BayanSettings bayanSettings,
        MappedFinancialStatement? mappedFinancialStatement, Statement statement, ILog log)
    {
        if (mappedFinancialStatement?.AccountBalances == null)
        {
            log.Info("Account Balances is null and no balances mapped.  Returning.");
            return;
        }

        log.Info("Will begin iteration through statement balances for mapping to the payload format.");

        foreach (var mappedBalance in mappedFinancialStatement.AccountBalances.Select(accountBalance => new Balance
                 {
                     Id = accountBalance.TargetFinancialTemplateId,
                     OriginValue = accountBalance.Balance,
                     OriginRounding = bayanSettings.OrigRounding,
                     IsModified = true,
                     Uda = false
                 }))
        {
            log.Debug($"Adding mapped balance to payload {JsonConvert.SerializeObject(mappedBalance)}");

            statement.Balances?.Add(mappedBalance);
        }

        log.Info($"Returning payload with {statement.Balances?.Count} balances.");
    }
}