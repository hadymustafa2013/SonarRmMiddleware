using System.Diagnostics;
using Data.Repository;
using Data.Repository.CreditLens;
using Data.Repository.POCO;
using Data.Repository.POCO.Documents.Allocation;
using Data.Repository.POCO.Documents.Document;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RmMiddleware.Bayan.Mapping.Models;
using RmMiddleware.Bayan.Responses;

namespace RmMiddleware.Bayan.Mapping;

public class Mapping(ILog log)
{
    public async Task<MappedFinancialStatement?> FetchStatementMapAndAggregate
        (BayanSettings bayanSettings, int entityId, int financialId, string? commercialRegistrationCode, int filingId)
    {
        log.Info($"Fetch Statement Map And Aggregate will lookup Entity Id {entityId}, Financial Id {financialId} and " +
                 $"Commercial Registration Code {commercialRegistrationCode}.  Will go to Bayan to get that statement.");

        var bayanStatement = await GetBayanStatementFromRepositoryInCreditLens(entityId, filingId, log);

        log.Info(
            $"Will proceed to fetch Entity Id {entityId}, Financial Id {financialId} and" +
            $" Commercial Registration Code {commercialRegistrationCode} from Bayan given Bayan Statement Id {bayanStatement?.Id} " +
            "available in CreditLens,  if not already fetched.");

        try
        {
            FinancialSummary? financialSummary;
            if (bayanStatement is { Fetched: false })
            {
                var sw = new Stopwatch();

                log.Info(
                    $"Entity Id {entityId}, Financial Id {financialId} and " +
                    $"Commercial Registration Code {commercialRegistrationCode} from Bayan given Bayan Statement Id {bayanStatement.Id} " +
                    " is not fetched and cached.  Will go to Bayan for this data.  Timer started.");

                financialSummary = await FetchFromBayan(bayanSettings, commercialRegistrationCode, filingId, sw,
                    bayanStatement);

                log.Info("Financial Summary has been fetched from Bayan. Some integrity checks will now take place.  " +
                         "Checking FIT which is where most of the payload to be mapped is.");

                if (financialSummary?.FIT == null)
                {
                    log.Info("Financial Summary has no FIT codes in the response.  Throwing exception.");
                    throw new Exception("No Accounting Codes found in FIT response.");
                }

                log.Info("Mapping summary data to the BayanStatement object to be updated in CreditLens.");

                bayanStatement.TotalAccountCountFetched = financialSummary.FIT.Count;
                bayanStatement.Payload = financialSummary.Payload?.ToString();
                bayanStatement.Fetched = true;
                bayanStatement.FetchedDate = DateTime.Now;
                bayanStatement.FetchedUser = "admin";
                bayanStatement.Pdf = financialSummary.PDF;
                bayanStatement.PdfFileName = financialSummary.FN;
                bayanStatement.PdfLink = "[RMIG]/api/bayan/pdf?entityId=" + entityId + "&filingId=" + filingId;
                bayanStatement.ResponseTime = (int)sw.ElapsedMilliseconds;
                
                log.Debug($"BayanStatement: {JsonConvert.SerializeObject(bayanStatement)}.");
            }
            else
            {
                if (bayanStatement?.Payload == null)
                {
                    log.Info(
                        "BayanStatement payload is empty which implies that it was not found in the Search from CreditLens.  Throwing exception.");

                    throw new Exception("Not Found.");
                }

                log.Info(
                    $"Entity Id {entityId}, Financial Id {financialId} and " +
                    $"Commercial Registration Code {commercialRegistrationCode} from Bayan given Bayan Statement Id {bayanStatement.Id} " +
                    $" is fetched and cached.  Cache payload is {bayanStatement.Payload}.  Will parse this as if it had just been received from Bayan.");

                financialSummary = Integration.MapStatementFromJObject(JObject.Parse(bayanStatement.Payload), log);
            }

            log.Info(
                "Has fetched the financial summary from either cache or Bayan new.  " +
                "Will fetch the account mappings specification from CreditLens.");
            
            log.Debug($"FinancialSummary: {JsonConvert.SerializeObject(financialSummary)}");

            var mappings = await GetSourceTargetAccountMappings();

            if (mappings == null)
            {
                throw new Exception("Could not acquire mapping definitions from CreditLens.  Mappings are null.");
            }

            log.Info($"Has fetched {mappings.Count} mappings from CreditLens.  " +
                     $"Will proceed to map them from Bayan source to target in the Financial Summary object.");

            var mapped = Map(financialSummary, mappings, log);

            if (mapped == null)
            {
                log.Info("Mapped object is null indicating missing, no data found or general failure in mapping.");

                throw new Exception("Not Found.");
            }

            if (mapped.AccountBalances == null)
            {
                log.Info(
                    "Mapped object is null indicating missing account balances, no data found or general failure in mapping.");

                throw new Exception("Not Found. Missing account balances.");
            }
            
            log.Info($"Total Mapped account balances is {mapped.AccountBalances.Count}.");

            bayanStatement.TotalAccountCountMatched = mapped.AccountBalances.Count;
            bayanStatement.SpreadCount += 1;
            bayanStatement.Success = true;
            bayanStatement.FinancialId = financialId;

            if (bayanStatement.Pdf != null)
            {
                if (bayanStatement.Pdf.Length == 0)
                {
                    log.Info("Nothing to upload as pdf bytes zero.");
                }
                else
                {
                    log.Info($"Is going to render {bayanStatement.Pdf.Length} base64 string for pdf.");

                    await RenderUploadAndAllocateDocument(bayanStatement, log);

                    log.Info($"Rendered {bayanStatement.Pdf.Length} base64 string and uploaded without error.");
                }
            }
            else
            {
                log.Error("Nothing to upload as pdf bytes empty.");
            }

            log.Info($"About to update the BayanStatement.");
            
            log.Debug($"BayanStatement: {JsonConvert.SerializeObject(bayanStatement)}");

            await BayanStatementRepository.Update(bayanStatement);

            log.Info($"Updated the BayanStatement for id {bayanStatement.Id}.  Returning");

            return mapped;
        }
        catch (Exception ex)
        {
            log.Error($"An exception in Fetch Statement Map And Aggregate has occured as {ex}");

            if (bayanStatement == null)
            {
                log.Error($"Can't update BayanStatement as the object is not prepared. Throwing error.");

                throw;
            }

            log.Error($"Including the error stack in the BayanStatement for update in CreditLens.");

            bayanStatement.ErrorStack = ex.ToString();

            await BayanStatementRepository.Update(bayanStatement);

            log.Info($"BayanStatement for updated in CreditLens and throwing the error.");

            throw;
        }
    }

    private static async Task RenderUploadAndAllocateDocument(BayanStatement bayanStatement, ILog log)
    {
        log.Info("Creating the weak wrapper to upload PDF to CreditLens documents.  " +
                 "Will create exception on authentication error.");

        var wrapper = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        log.Info("Authenticated the weak wrapper will now attempt upload.");

        var location = Environment.GetEnvironmentVariable("BAYAN_DOCUMENT_UPLOAD_LOCATION") ?? "4.1";

        log.Info($"Document will be uploaded to location {location}.  Passing authentication to upload.");

        var document = await RenderAndUploadDocument(bayanStatement, wrapper, location, log);

        log.Info(
            $"Rendered and uploaded document to location {location}.  " +
            $"This must be allocated in a another step subject to the document returned not being null.");

        if (document != null)
        {
            log.Info(
                "Document has been returned on upload.  Now allocating as the two step file upload processing CreditLens.");

            await AllocateUploadedDocument(document, bayanStatement, wrapper, location);
            
            log.Info("Allocated document to a domain model in CreditLens.");
        }
        else
        {
            log.Info("Document has been not returned on upload indicating an error.");
        }
    }

    private static async Task<DocumentPayloadWrapper?> RenderAndUploadDocument(BayanStatement bayanStatement,
        WeakWrapper wrapper, string location, ILog log)
    {
        log.Info("Starting the render and upload process.  Checking if the pdf Base 64 is null.");

        if (bayanStatement.Pdf == null)
        {
            log.Info("Base64 is null and nothing to render..");
            return null;
        }

        log.Info("Will convert base64 into bytes.");

        var bytes = Convert.FromBase64String(bayanStatement.Pdf);
        var uri =
            $"/api/document/batchUpload/{location}/00000000-0000-0000-0000-000000000000/00000000-0000-0000-0000-000000000000";

        log.Info($"Final document is {bytes.Length}. Uploading bytes as post file to {uri}.");

        var ms = new MemoryStream(bytes);
        var document = (await wrapper.PostFileOnly(uri,
            bayanStatement.PdfFileName ?? "Default.pdf", ms)).ToObject<DocumentPayloadWrapper>();

        log.Info($"{bytes.Length} uploaded as post file to {uri}.  Returning.");

        return document;
    }

    private static async Task AllocateUploadedDocument(DocumentPayloadWrapper? document, BayanStatement bayanStatement,
        WeakWrapper wrapper, string location)
    {
        var allocationPayload = new AllocationPayload();
        if (document is { payLoad: not null })
        {
            allocationPayload.DocumentId = document.payLoad[0].DocumentId;
            allocationPayload.UploadStatus = 1;
            allocationPayload.Title = bayanStatement.PdfFileName;
            allocationPayload.OriginalFileName = bayanStatement.PdfFileName;
            allocationPayload.Category = location;
            allocationPayload.OperationType = "Create";
            allocationPayload._uniqueRowId = 1;
            allocationPayload.BaseVersionId_ = 0;
            allocationPayload.VersionId_ = 1;
        }

        var allocationPayloadWrapper = new AllocationPayloadWrapper
        {
            payLoad = new List<AllocationPayload> { allocationPayload }
        };

        await wrapper.PostStringBodyOnly(
            "/api/document/batchUpdate/", JsonConvert.SerializeObject(allocationPayloadWrapper));
    }

    private async Task<FinancialSummary?> FetchFromBayan(BayanSettings bayanSettings,
        string? commercialRegistrationCode, int filingId,
        Stopwatch sw,
        BayanStatement bayanStatement)
    {
        sw.Start();
        log.Info("Fetch From Bayan started timer.  Will now get the Financial Summary and update the BayanStatement.");

        FinancialSummary? financialSummary;
        try
        {
            financialSummary = await GetFinancialSummary(bayanSettings, commercialRegistrationCode, filingId);

            log.Info($"Fetched financial summary.");
            
            log.Debug($"{JsonConvert.SerializeObject(financialSummary)}");
        }
        catch (Exception ex)
        {
            log.Error($"Exception {ex} on fetching from Bayan with Commercial Registration Code " +
                     $"{commercialRegistrationCode} and Filing Id {filingId}. " +
                     $"Logging the error in the BayanStatement domain repository.");

            sw.Stop();

            bayanStatement.ErrorStack = ex.ToString();
            bayanStatement.ResponseTime = (int)sw.ElapsedMilliseconds;

            await BayanStatementRepository.Update(bayanStatement);

            log.Error($"Logged exception for " +
                     $"{commercialRegistrationCode} and Filing Id {filingId} in the BayanStatement domain repository.  " +
                     $"Throwing error.");

            throw;
        }

        sw.Stop();
        return financialSummary;
    }

    private static async Task<BayanStatement?> GetBayanStatementFromRepositoryInCreditLens(int entityId, int filingId,
        ILog log)
    {
        var search = new Dictionary<string, object>
        {
            { "FilingId", filingId },
            { "EntityId", entityId }
        };
        
        log.Info("Invoking search in Bayan Statement Repository.");

        log.Debug($"Search: {JsonConvert.SerializeObject(search)}");
        
        var bayanStatements = await BayanStatementRepository.Search(search);

        log.Info($"Bayan Statement Repository returned {bayanStatements.Count} statement(s).");

        if (bayanStatements.Count == 0 || bayanStatements.FirstOrDefault() == null)
        {
            log.Info($"Given no statements an exception thrown.");

            throw new Exception("Not found a statement.");
        }

        var bayanStatement = bayanStatements.FirstOrDefault();

        log.Info($"Bayan Statement Repository returning id {bayanStatement?.Id}.");

        return bayanStatement;
    }

    private static MappedFinancialStatement Map(FinancialSummary? financialSummary,
        IReadOnlyCollection<SourceTargetAccountMapping> mappings, ILog log)
    {
        log.Info("Crating mapped financial statement parent object to house the balances.  " +
                 "Date will be checked and throw exception if can't parse.");

        var value = new MappedFinancialStatement
        {
            StatementDate = DateTime.Parse(financialSummary?.DAT ??
                                           throw new InvalidOperationException(
                                               "DAT, Statement Date, is empty.  This is an indication of no data having been found.")),
            Currency = financialSummary.CC,
            AccountBalances = new List<AccountBalance>(),
            Type = financialSummary.TYPE,
            Consolidated = financialSummary.CS
        };

        log.Info("Validating that the FIT object is not null.");

        if (financialSummary.FIT == null)
        {
            log.Info("FIT object is null, return null.");
            return value;
        }

        foreach (var fit in financialSummary.FIT)
        {
            log.Info($"Mapping {fit.ITEM_COD} and {fit.DOC_COD} will look for match.");

            var sourceTargetAccountMapping = MatchSourceTargetAccountMapping(mappings, fit, log);

            if (sourceTargetAccountMapping != null)
            {
                log.Info($"Mapping {fit.ITEM_COD} and {fit.DOC_COD} matched " +
                         $"and will now map to spreading object which when serialized " +
                         $"is something that can be processed by CreditLens.");

                AddOrMergeAggregateMatchedEntry(value.AccountBalances, sourceTargetAccountMapping, fit,log);

                log.Info($"Mapping {fit.ITEM_COD} and {fit.DOC_COD} has mapped to account balances list.");
            }
            else
            {
                log.Info($"Mapping {fit.ITEM_COD} and {fit.DOC_COD} unmatched.");
            }
        }

        return value;
    }

    private static void AddOrMergeAggregateMatchedEntry(List<AccountBalance> accountBalances,
        SourceTargetAccountMapping? sourceTargetAccountMapping, FIT fit,ILog log)
    {
        log.Info($"Values to merge to account balances are " +
                 $"Target Financial Template Id {sourceTargetAccountMapping?.TargetFinancialTemplateId} and " +
                 $"Balance {fit.EV} before Source Coefficient.  " +
                 $"Looking up the Target Financial TemplateId from those available in account balances already to merge on.");
        
        if (sourceTargetAccountMapping == null)
        {
            log.Info("Can't proceed as source account mapping is null.  Method returning.");
            
            return;  
        }

        log.Info($"Will multiply EV {fit.EV} by Source Coefficient to arrive at the balance value {sourceTargetAccountMapping.SourceCoefficient}.");
        
        var value = fit.EV * sourceTargetAccountMapping.SourceCoefficient;
        
        log.Info($"Balance value is {value}. Looking up existing account balance for {sourceTargetAccountMapping.TargetFinancialTemplateId}");
        
        var accountBalance = accountBalances.Find(f =>
            f.TargetFinancialTemplateId == sourceTargetAccountMapping.TargetFinancialTemplateId);

        if (accountBalance == null)
        {
            log.Info("Not Found an existing account balance. Creating a new account balance.");
            
            accountBalances.Add(new AccountBalance
            {
                TargetFinancialTemplateId = sourceTargetAccountMapping.TargetFinancialTemplateId,
                Balance = value,
                Count = 1,
                TargetItemDescription = sourceTargetAccountMapping.TargetItemDescription
            });
            
            log.Debug($"AccountBalance: {JsonConvert.SerializeObject(accountBalance)}.");
        }
        else
        {
            accountBalance.Count += 1;
            accountBalance.Balance += value;
            
            log.Info($"Found account balance, incremented and set value.");
            
            log.Debug($"{JsonConvert.SerializeObject(accountBalance)}{JsonConvert.SerializeObject(accountBalance)}");
        }
    }

    private static SourceTargetAccountMapping? MatchSourceTargetAccountMapping(
        IEnumerable<SourceTargetAccountMapping> mappings, FIT fit, ILog log)
    {
        log.Info($"Matching DOC_COD of {fit.DOC_COD} and ITEM_COD of {fit.ITEM_COD}.");

        var sourceTargetAccountMappings = mappings.Where(w =>
                w.SourceDocCode == fit.DOC_COD
                && w.SourceItemCode == fit.ITEM_COD)
            .OrderBy(s => s.EvaluationPriority).ToList();

        log.Info($"Found match count {sourceTargetAccountMappings.Count}.  Testing if has invalid SourceCoefficient");

        if (!sourceTargetAccountMappings.Any(a => a.SourceCoefficient < 0))
        {
            log.Info("Fetching first matched account");

            var sourceTargetAccountMapping = sourceTargetAccountMappings.FirstOrDefault();

            if (sourceTargetAccountMapping == null)
            {
                log.Info("First matched account is null.  Returning.");
                
                return null;
            }

            log.Info("Fetched the first matched account.  Will test SourceCoefficient = 0 to infer a header record.");

            sourceTargetAccountMapping.DocCodeHeader = sourceTargetAccountMapping.SourceCoefficient == 0;

            log.Info($"Header record {sourceTargetAccountMapping.DocCodeHeader}.  Returning match.");

            return sourceTargetAccountMapping;
        }

        log.Info($"Unmatched {fit.ITEM_COD} {fit.DOC_COD} or has matched with a SourceCoefficient of less than zero.  " +
                 $"Handling that special case.  If a positive value then map to the MMAS account where SourceCoefficient is 1.  " +
                 $"This logic exists because there are some cases where positive " +
                 $"and negative values need to be mapped to different MMAS accounts,  so,  there may be duplicates.");

        if (fit.EV < 0)
        {
            log.Info("The balance value from Bayan is less than 0 which implies it should mapped to the MMAS account for which source coefficient is negative 1.");
            
            return sourceTargetAccountMappings.FirstOrDefault(f
                => f is { SourceCoefficient: < 0 });
        }
        
        log.Info("The balance value from Bayan is not negative which implies it should mapped to the MMAS account for which source coefficient is  1");
        
        return sourceTargetAccountMappings.FirstOrDefault(f
            => f is { SourceCoefficient: > 0 });
    }

    private static async Task<List<SourceTargetAccountMapping>?> GetSourceTargetAccountMappings()
    {
        return await SourceTargetAccountMappingRepository.Get();
    }

    private async Task<FinancialSummary?> GetFinancialSummary(BayanSettings bayanSettings,
        string? commercialRegistrationCode, int filingId)
    {
        log.Info("Creating the Bayan Integration instance.");

        var integration = new Integration(
            new Uri(bayanSettings.BayanHttpEndpointJwt ?? "http://localhost:5079"),
            new Uri(bayanSettings.BayanHttpEndpointReport ?? "http://localhost:5079/CommercialCreditReport/1.0/"),
            bayanSettings.BayanHttpUser ?? "admin",
            bayanSettings.BayanHttpPassword ?? "admin", log,bayanSettings.BayanAuthentication,bayanSettings.BayanChannelIdHeader);

        log.Info("In the absence of an error it can be taken that CreditLens is connected.");

        var financialSummary = await integration.GetStatementByFinancialId(commercialRegistrationCode, filingId, log);
        return financialSummary;
    }
}