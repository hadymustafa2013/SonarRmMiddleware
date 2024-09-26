using System.Diagnostics;
using System.Net;
using Data.Repository;
using Data.Repository.Dto;
using Data.Repository.POCO;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RmMiddleware.Bayan.Responses;
using RmMiddleware.Helpers;

namespace RmMiddleware.Bayan;

public class Integration
{
    private readonly Uri _baseUriReport;
    private readonly Uri _baseUriJwt;
    private Dictionary<string, string?>? _headers;
    private readonly ILog _log;

    public Integration(Uri baseUriJwt, Uri baseUriReport, string user, string password, ILog log
        ,bool authenticate,bool channelIdHeader)
    {
        _log = log;
        _baseUriReport = baseUriReport;
        _baseUriJwt = baseUriJwt;

        if (authenticate)
        {
            _headers = AuthenticateWithUserPasswordAndGetJwtForFutureUse(user, password);
            
            log.Info("Had added authentication header from Bayan.");
        }
        else
        {
            _headers = new Dictionary<string, string?>();   
            
            log.Info("Has not authenticated with Bayan.");
        }

        if (!channelIdHeader) return;
        
        _headers.Add("ChannelID","Credit Lens");
            
        log.Info("Has added ChannelId=Credit Lens to HTTP Headers.");
    }

    private Dictionary<string, string?> AuthenticateWithUserPasswordAndGetJwtForFutureUse(string user,
        string password)
    {
        _log.Info($"Authenticating CreditLens with user name {user} and redacted password.  Creating header.");

        var headers = new Dictionary<string, string?>
        {
            { "Authorization", HttpClientHelper.BasicAuthenticationHeaderString(user, password) }
        };

        var authenticationEndpointUri = new Uri(_baseUriJwt, "getJsonWebToken");

        _log.Info($"Authenticating CreditLens with user name {user} will POST to url {authenticationEndpointUri}.  " +
                  $"Anything other than 200 will throw exception.");

        var authentication = HttpClientHelper.GetReturnJObject(authenticationEndpointUri, headers)
            .Result?.ToObject<Authentication>();

        _log.Info($"Authenticating CreditLens with user name {user} has returned 200. " +
                  $"Creating header for reuse with the CreditLens bearer token.  CreditLens JWT:{authentication?.Jwt}");

        _headers = new Dictionary<string, string?>
        {
            { "Authorization", $"Bearer {authentication?.Jwt}" }
        };

        _log.Info($"Authenticating CreditLens with user name {user} returning from authentication.");

        return _headers;
    }

    public async Task<FinancialSummary?> GetStatementByFinancialId(string? commercialRegistrationCode, int filingId,
        ILog log)
    {
        var uri = new Uri(_baseUriReport, "CB_ME_Product");

        log.Info($"Constructing the POST body to query the product for " +
                 $"commercial registration code {commercialRegistrationCode} and filing id  {filingId}.  Will POST to {uri}.");

        var requestBody = GetByFilingIdRequestString(commercialRegistrationCode, filingId);

        log.Info($"Constructing the POST body to query is {JsonConvert.SerializeObject(requestBody)}.  Will POST.  " +
                 $"Anything other than a 200 will cause an error.");

        var jObject = await HttpClientHelper
            .PostStringBodyReturnJObject(uri, requestBody, _headers);

        log.Info($"Credit Report response payload is {jObject}.");

        log.Info($"Will move to perform mapping to proper objects on the basis of the Bayan response.");

        var financialSummary = MapStatementFromJObject(jObject, log);

        log.Info($"Returning the Financial Summary.");
        
        log.Debug($"FinancialSummary: {JsonConvert.SerializeObject(financialSummary)}");

        return financialSummary;
    }

    public static FinancialSummary? MapStatementFromJObject(JObject? jObject, ILog log)
    {
        log.Info($"Will map the data by traversing the JObject.  " +
                 $"No integrity checks will take place except existence of the json element, " +
                 $"the absence of which would cause an exception. Returning the financial information.");

        var financialInfoJToken = GetFinInfoFromJObject(jObject);

        log.Info($"Returning the financial summary from financial information.  " +
                 $"Checking to see if the FIN_SUM element exists.");
        
        log.Debug($"FinancialInfoJToken: {financialInfoJToken}.");

        if (financialInfoJToken?["FIN_SUM"] == null)
        {
            throw new Exception("The response from Bayan does not have financial summary available (FIN_SUM).");
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var financialSummaryJToken = financialInfoJToken["FIN_SUM"].Type == JTokenType.Array
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            ? financialInfoJToken["FIN_SUM"]?[0]
            : financialInfoJToken["FIN_SUM"];

        log.Info($"Returning the financial statement from financial summary.  " +
                 $"Checking if FIN_STAT element exists.");
        
        log.Debug($"FinancialSummaryJToken: {financialSummaryJToken}");

        if (financialInfoJToken["FIN_STAT"] == null)
        {
            throw new Exception("The response from Bayan does not have financial statement available (FIN_STAT).");
        }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var financialStatementJToken = financialInfoJToken["FIN_STAT"].Type == JTokenType.Array
            ?
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            financialInfoJToken["FIN_STAT"]?[0]
            : financialInfoJToken["FIN_STAT"];

        if (financialSummaryJToken?["FIT"] == null)
        {
            throw new Exception("The response from Bayan does not have financial statement accounts available (FIT).");
        }

        log.Info($"Returned the financial information." +
#pragma warning disable CS8604 // Possible null reference argument.
                 $"There are {financialSummaryJToken["FIT"].Count()} entries, only looking at first.  Checking if FIT element exists");
#pragma warning restore CS8604 // Possible null reference argument.

        log.Debug($"FinancialStatementJToken: {financialStatementJToken}");
        
        var financialStatementFitAggregationJToken = financialSummaryJToken["FIT"]?.Type != JTokenType.Array
            ? financialSummaryJToken["FIT"]
            : financialSummaryJToken["FIT"]?[0];

        log.Info(
            $"Returned the first FIT element.  Will now get FIT record to map.");
        
        log.Debug($"FinancialStatementFitAggregationJToken: {financialStatementFitAggregationJToken}");

        var financialStatementFitJToken = financialStatementJToken?["FIT"];

        log.Info($"Returned the final FIT record. Returning the pdf from financial summary.");
        
        log.Debug($"FinancialStatementFitJToken: {financialStatementFitJToken}");

        var financialStatementPdf = financialStatementJToken?["PDF"];

        log.Info($"Returned the PDF. " +
                 $"Map Statement From JObject was able to extract the required elements to create " +
                 "the Financial Summary object.  Will create the Financial Summary object.");
        
        log.Debug($"FinancialStatementPdf: {financialStatementPdf}");

        var financialSummary = MapAllExtractedJsonTokensToTheFinancialSummaryObject(jObject, financialSummaryJToken,
            financialStatementJToken,
            financialStatementFitAggregationJToken, financialStatementPdf);

        log.Debug($"Financial Summary: {JsonConvert.SerializeObject(financialSummary)}.  ");
                 
        log.Info($"Will loop through the account balances and map to the financial summary object.");

        financialSummary = MapAccountBalancesToFinancialSummary(financialStatementFitJToken, financialSummary, log);

        log.Info($"Has fully mapped the Bayan response to a strong object for processing.  " +
                 $"At this stage no integrity checking has taken place except to map the values across, subject to the " +
                 $"elements existing in the json.");

        return financialSummary;
    }

    private static async Task<BayanStatementRequest> UpsertEntityBayanCacheForSuccess(
        BayanStatementRequest bayanStatementRequest, Stopwatch sw, JObject? jObject, ILog log)
    {
        bayanStatementRequest.ResponseTime = sw.ElapsedMilliseconds;
        bayanStatementRequest.Payload = jObject?.ToString();
        bayanStatementRequest.Success = true;

        log.Info($"Will proceed to upsert {JsonConvert.SerializeObject(bayanStatementRequest)}");

        var search = new Dictionary<string, object>();
        if (bayanStatementRequest.EntityId != null)
        {
            search.Add("EntityId", bayanStatementRequest.EntityId.Value);
            log.Info(
                $"Has Entity Id {bayanStatementRequest.EntityId} for the upsert in Bayan Statement Request Repository.");
        }

        log.Info("Is about in Bayan Statement Request Repository. Will return or throw exception.");

        return await BayanStatementRequestRepository.Upsert(bayanStatementRequest, search);
    }

    private static async Task<BayanStatementRequest> UpsertEntityBayanCacheForError(
        BayanStatementRequest bayanStatementRequest, Stopwatch sw, JObject? jObject, string errorStack)
    {
        bayanStatementRequest.ResponseTime = sw.ElapsedMilliseconds;
        bayanStatementRequest.Payload = jObject?.ToString();
        bayanStatementRequest.ErrorStack = errorStack;

        var search = new Dictionary<string, object>();
        if (bayanStatementRequest.EntityId != null) search.Add("EntityId", bayanStatementRequest.EntityId.Value);
        return await BayanStatementRequestRepository.Upsert(bayanStatementRequest, search);
    }

    private static FinancialSummary? MapAccountBalancesToFinancialSummary(JToken? financialStatementFitJTokens,
        FinancialSummary? financialSummary, ILog log)
    {
        if (financialStatementFitJTokens == null)
        {
            log.Info("There are no FIT tokens as it is null.");

            return financialSummary;
        }

        log.Info($"There are {financialStatementFitJTokens.Count()} to iterate though from the payload.");
        foreach (var financialStatementFitJToken in financialStatementFitJTokens)
        {
            log.Debug($"FinancialStatementFitJToken: {JsonConvert.SerializeObject(financialStatementFitJToken)}.");

            var fit = new FIT
            {
                DOC_COD = financialStatementFitJToken["DOC_COD"]?.ToString(),
                DOC_DESC = financialStatementFitJToken["DOC_DESC"]?.ToString(),
                ITEM_COD = financialStatementFitJToken["ITEM_COD"]?.ToString(),
                ITEM_DESC = financialStatementFitJToken["ITEM_DESC"]?.ToString(),
                EC = financialStatementFitJToken["EC"]?.ToString(),
                SRT = financialStatementFitJToken["SRT"]?.ToString(),
                EV = long.TryParse(financialStatementFitJToken["EV"]?.ToString() ?? "0", out var ev) ? ev : 0
            };

            log.Info($"Adding to the list of balances with DOC_COD {fit.DOC_COD}, ITEM_COD {fit.ITEM_COD} and balance {fit.EV}.");
            
            log.Debug($"FIT: {JsonConvert.SerializeObject(fit)}");
            
            financialSummary?.FIT?.Add(fit);
        }

        log.Info($"Returning {financialSummary?.FIT?.Count} records mapped for FIT.");

        return financialSummary;
    }

    private static FinancialSummary MapAllExtractedJsonTokensToTheFinancialSummaryObject(JObject? jObject,
        JToken? financialSummaryJToken,
        JToken? financialStatementJToken,
        JToken? financialStatementFitAggregationJTokens,
        JToken? financialStatementPdfJToken)
    {
        var financialSummary = new FinancialSummary
        {
            Payload = jObject,
            CC = financialSummaryJToken?["CC"]?.ToString(),
            CC_DESC = financialSummaryJToken?["CC_DESC"]?.ToString(),
            CS = financialSummaryJToken?["CS"]?.ToString().Equals("true",StringComparison.CurrentCultureIgnoreCase),
            DAT = financialSummaryJToken?["DAT"]?.ToString(),
            BID = financialStatementJToken?["BID"]?.ToString(),
            FID = financialStatementJToken?["FID"]?.ToString(),
            TYPE = financialStatementJToken?["TYPE"]?.ToString(),
            AMN = financialStatementJToken?["AMN"]?.ToString(),
            AO = financialStatementJToken?["AO"]?.ToString(),
            IA = financialStatementJToken?["IA"]?.ToString().Equals("true",StringComparison.CurrentCultureIgnoreCase),
            VER = financialStatementJToken?["VER"]?.ToString(),
            FIT = new List<FIT>(),
            PDF = financialStatementPdfJToken?["*body"]?.ToString(),
            FN = financialStatementPdfJToken?["FN"]?.ToString(),
            REV = long.TryParse(financialStatementFitAggregationJTokens?["REV"]?.ToString() ?? "0", out var rev)
                ? rev
                : 0,
            NET_INC = long.TryParse(financialStatementFitAggregationJTokens?["NET_INC"]?.ToString() ?? "0",
                out var netInc)
                ? netInc
                : 0,
            TOT_ASSET = long.TryParse(financialStatementFitAggregationJTokens?["TOT_ASSET"]?.ToString() ?? "0",
                out var totAsset)
                ? totAsset
                : 0,
            TOT_LIAB = long.TryParse(financialStatementFitAggregationJTokens?["TOT_LIAB"]?.ToString() ?? "0",
                out var totLiab)
                ? totLiab
                : 0
        };

        return financialSummary;
    }

    private static JToken? GetFinInfoFromJObject(JObject? jObject)
    {
        return jObject?["CB_ME_ProductResponse"]?["ProductResponse"]?[0]?["CB_ME_ProductOutput"]?["B2BResponse"]?[
                "Product"]?
            ["BIRDATA"]?["FIN_INFO"];
    }

    private static string GetByFilingIdRequestString(string? commercialRegistrationCode, int filingId)
    {
        var jObject = GetRequestObject();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        jObject["Product"][0]["CB_ME_ProductInput"]["GeneralInfo"]["ConsentFlag"] = "1";
        jObject["Product"][0]["CB_ME_ProductInput"]["SubjectCodes"]["CommercialRegistrationCode"] =
            commercialRegistrationCode;
        jObject["Product"][0]["CB_ME_ProductInput"]["B2BInfo"]["ProductID"] = "RPT_HQR_QWM_PDF";
        jObject["Product"][0]["CB_ME_ProductInput"]["B2BInfo"]["Culture"] = "EN";
        jObject["Product"][0]["CB_ME_ProductInput"]["B2BInfo"]["Financial"][0]["FilingID"] = filingId;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return jObject.ToString();
    }

    public async Task<BayanStatementRequestDto> SearchCompanyInfo(int entityId)
    {
        _log.Info($"Bayan Integration Search Company Info invoked for Entity Id {entityId}. " +
                  $"Will invoke Lookup Commercial Registration Code Given Entity Id And Verify Consent for Entity " +
                  $"repository. Needs the commercial registration code but consent will be validated in the same process.");

        var commercialRegistrationCode =
            await EntityRepository.LookupCommercialRegistrationCodeGivenEntityIdAndVerifyConsent(entityId);

        _log.Info($"Bayan Integration Search Company Info invoked for Entity Id {entityId}. " +
                  $"has found Commercial Registration Code {commercialRegistrationCode}.");

        var returnValue = new BayanStatementRequestDto();
        var sw = BuildAndStartStopwatch();

        _log.Info($"Bayan Integration Search Company Info invoked for Entity Id {entityId} and . " +
                  $"CommercialRegistrationCode {commercialRegistrationCode}.  Has started timer.  Will " +
                  $"Call Search Company Info Endpoint.");

        var bayanStatementRequest = BuildBayanStatementRequest(entityId);
        try
        {
            var jObject = await CallSearchCompanyInfoEndpoint(commercialRegistrationCode, returnValue,
                bayanStatementRequest, sw, _log);

            _log.Info($"Bayan Integration Search Company Info has returned a JObject {jObject}. will pass to Map the " +
                      "JObject for the available statements and upsert them in BayanStatement in CreditLens.");

            await MapJObjectAndUpsertBayanStatement(entityId, jObject, returnValue, _log);

            _log.Info(
                "Has mapped the JObject and inserted if not exists into BayanStatement in CreditLens.");
        }
        catch (WebException ex)
        {
            _log.Error($"An exception has been created in Search Company Info as {ex}. " +
                       $"Will update the BayanStatementRequest object detailing this error.");

            returnValue.BayanStatementRequest =
                await UpsertEntityBayanCacheForError(bayanStatementRequest, sw, null, ex.ToString());

            _log.Error(
                $"Updated the BayanStatementRequest object detailing this error.  Throwing the error to kill the request.");

            throw;
        }

        _log.Info("Done with the SearchCompanyInfo method.  Returning.");

        return returnValue;
    }

    private static async Task MapJObjectAndUpsertBayanStatement(int companyId, JObject? jObject,
        BayanStatementRequestDto returnValue,
        ILog log)
    {
        log.Info("Extracting CompanyInformation from JObject.");
        var companyInformationJToken = GetCompanyInformationJObject(jObject);
        log.Info($"Extracted {companyInformationJToken}.");

        log.Info("Creating and mapping Company information.");
        var companyInformation = MapCompanyInformation(companyInformationJToken);
        log.Info($"Has mapped the CompanyInformation.");
        log.Debug($"CompanyInformation: {JsonConvert.SerializeObject(companyInformation)}");

        log.Info("Getting the financial aggregation.");
        var companyItemFinancialJToken = GetCompanyItemFinancialFromJToken(companyInformationJToken);
        log.Info($"Extracted financial aggregation.");
        log.Debug($"CompanyItemFinancialJToken: {JsonConvert.SerializeObject(companyItemFinancialJToken)} {JsonConvert.SerializeObject(companyItemFinancialJToken)}");

        log.Info($"Will map the financials to a list.");
        var companyItemFinancials
            = MapAvailableFinancialStatements(companyInformation, companyItemFinancialJToken).CompanyFinancials;
        
        log.Info($"Has mapped company financials.");
        log.Debug($"CompanyItemFinancials: {JsonConvert.SerializeObject(companyItemFinancials)}");

        if (companyItemFinancials !=
            null)
        {
            log.Info($"Company financials will be looped through and inserted to CreditLens as not null, " +
                     $"it has {companyItemFinancials.Count} values.");

            foreach (var bayanStatement in companyItemFinancials.Select(companyItemFinancial => new BayanStatement
                     {
                         StatementDate = companyItemFinancial.Annuity,
                         FilingId = companyItemFinancial.FilingId,
                         EntityId = companyId,
                         Resubmission = companyItemFinancial.IsResubmission,
                         ResubmissionComments = companyItemFinancial.ResubmissionComments,
                         BayanStatementRequestId = companyId
                     }))
            {
                returnValue.BayanStatements.Add(bayanStatement);

                log.Info(
                    $"Has created a bayan statement for insert. Added to response payload.");
                log.Debug($"BayanStatement: {JsonConvert.SerializeObject(bayanStatement)}");

                var upsertSearch = new Dictionary<string, object> { { "EntityId", companyId } };
                if (bayanStatement.FilingId != null)
                {
                    upsertSearch.Add("FilingId", bayanStatement.FilingId);

                    log.Info($"The Filing Id of {bayanStatement.FilingId} is the key for update.");
                }

                else
                {
                    log.Info($"The Filing Id is null so will be inserted.");
                }

                if (bayanStatement.StatementDate?.Year > 2019)
                {
                    log.Info($"Is about to call BayanStatementRepository repository Upsert for {bayanStatement.FilingId}.");

                    await BayanStatementRepository.InsertIfNotExists(bayanStatement, upsertSearch);

                    log.Info($"BayanStatementRepository repository Upsert done for {bayanStatement.FilingId}.");   
                }
                else
                {
                    log.Info($"BayanStatementRepository repository not inserted as statement date of {bayanStatement.StatementDate?.Year} on or before 2019.");   
                }
            }
        }
    }

    private async Task<JObject?> CallSearchCompanyInfoEndpoint(string? commercialRegistrationCode,
        BayanStatementRequestDto returnValue,
        BayanStatementRequest bayanStatementRequest, Stopwatch sw, ILog log)
    {
        log.Info("Is preparing request for Call Search Company Info Endpoint.");

        var searchCompanyInfoEndpointUri =
            new Uri(_baseUriReport, $"SearchCompanyInfo?CompanyID={commercialRegistrationCode}");

        log.Info($"Request url for Call Search Company Info Endpoint is {searchCompanyInfoEndpointUri}.  " +
                 $"Will call GET on it and return a JObject for parsing.  Anything other than a 200 will throw exception.");

        var jObject = await HttpClientHelper.GetReturnJObject(searchCompanyInfoEndpointUri, _headers);

        log.Info(
            $"Request url for Call Search Company Info Endpoint is {searchCompanyInfoEndpointUri} " +
            $"has returned json {jObject}.");

        log.Info(
            "Given absence of an error will proceed to call CreditLens and insert or update the response payload.");

        try
        {
            returnValue.BayanStatementRequest =
                await UpsertEntityBayanCacheForSuccess(bayanStatementRequest, sw, jObject, log);
        }
        catch (Exception ex)
        {
            log.Error($"Exception in Call Search Company Info Endpoint as {ex}");

            throw;
        }

        log.Info($"Returning CallSearchCompanyInfoEndpoint.");

        return jObject;
    }

    private static Stopwatch BuildAndStartStopwatch()
    {
        var sw = new Stopwatch();
        sw.Start();
        return sw;
    }

    private static BayanStatementRequest BuildBayanStatementRequest(int companyId)
    {
        var bayanStatementRequest
            = new BayanStatementRequest
            {
                RequestDate = DateTime.Now,
                RequestUser = "admin",
                EntityId = companyId,
                Id = companyId
            };
        return bayanStatementRequest;
    }

    private static CompanyInformation MapAvailableFinancialStatements(CompanyInformation companyInformation,
        JToken? companyItemFinancialJToken)
    {
        companyInformation.CompanyFinancials = new List<CompanyItemFinancial>();
        if (companyItemFinancialJToken == null) return companyInformation;
        foreach (var companyItemFinancialToken in companyItemFinancialJToken)
        {
            var companyItemFinancial = new CompanyItemFinancial
            {
                FilingId = (int)(companyItemFinancialToken["FilingID"] ?? 0),
                Annuity =
                    DateTime.Parse(companyItemFinancialToken["Annuity"]?.ToString().Split("/")[2] ?? string.Empty),
                NatureCode = (int)(companyItemFinancialToken["NatureCode"] ?? 0),
                NatureDescription = companyItemFinancialToken["NatureDescription"]?.ToString(),
                IsResubmission = (bool)(companyItemFinancialToken["IsResubmission"] ?? false),
                ResubmissionComments = companyItemFinancialToken["ResubmissionComments"]?.ToString()
            };

            companyInformation.CompanyFinancials.Add(companyItemFinancial);
        }

        return companyInformation;
    }

    private static JToken? GetCompanyItemFinancialFromJToken(JToken? companyInformationJToken)
    {
        var companyItemFinancialJToken = companyInformationJToken?["CompanyFinancials"]?["CompanyItemFinancial"];
        return companyItemFinancialJToken;
    }

    private static CompanyInformation MapCompanyInformation(JToken? companyInformationJToken)
    {
        var companyInformation = new CompanyInformation
        {
            CrNumber = companyInformationJToken?["CRNumber"]?.ToString(),
            CompanyNameArabic = companyInformationJToken?["CompanyNameArabic"]?.ToString(),
            NationalNumber = companyInformationJToken?["NationalNumber"]?.ToString(),
            CrMainNumber = companyInformationJToken?["CRMainNumber"]?.ToString(),
            CompanyUnitTypeCode = companyInformationJToken?["CompanyUnitTypeCode"]?.ToString(),
            CompanyUnitTypeDescription = companyInformationJToken?["CompanyUnitTypeDescription"]?.ToString(),
            FinancialStatementAvailable = (bool)(companyInformationJToken?["FinancialStatementAvailable"] ?? false)
        };
        return companyInformation;
    }

    private static JToken? GetCompanyInformationJObject(JObject? jObject)
    {
        var companyInformationJToken = jObject?["SearchCompanyInfoResponse"]?["CompanyList"]?["CompanyInformation"];
        return companyInformationJToken;
    }

    private static JObject GetRequestObject()
    {
        var json = @"{
                ""Product"" : [{
                    ""CB_ME_ProductInput"" : {
                    ""GeneralInfo"" : {
                        ""ConsentFlag"" : """"
                        },
                    ""SubjectCodes"" : {
                        ""ProviderSubjectNo"" : """",
                        ""CBSubjectCode"" : """",
                        ""NationalIDCode"" : """",
                        ""CommercialRegistrationCode"" : """",
                        ""NationalNumber"" : """"
                            },
                    ""B2BInfo"" : {
                        ""Financial"" : [ {
                            ""FilingID"" : """",
                            ""Status"" : """",
                            ""Year"" : """",
                            ""Nature"" : """"
                                }],
                        ""Culture"" : """",
                        ""ProductID"" : """"
                        }
                    }
                }]
            }";

        return JObject.Parse(json);
    }
}