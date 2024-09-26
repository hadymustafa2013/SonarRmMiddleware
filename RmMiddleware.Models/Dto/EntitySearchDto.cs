namespace RmMiddleware.Models.DTO;

public class CreditLensSearchResponse
{
    public IEnumerable<Dictionary<string, object>>? PayLoad { get; set; }
    public Status? Status { get; set; }
}

public class Status
{
    public Info[]? Info { get; set; }
    public Warn[]? Warn { get; set; }
    public Error[]? Error { get; set; }
}

public class Info
{
    public bool IsOverMax { get; set; }
    public int ResultCount { get; set; }
}

public class Error
{
    public string? ResourceId { get; set; }
    public bool ShowMessage { get; set; }
}

public class Payload
{
    public int EntityId { get; set; }
    public string? LongName { get; set; }
    public string? ShortName { get; set; }
    public string? EntityType { get; set; }
    public string? CountryOfInc { get; set; }
    public string? PrimaryBankingOfficer { get; set; }
    public string? PrimaryCreditOfficer { get; set; }
    public Industrycode[]? IndustryCode { get; set; }
    public string? IndClassification { get; set; }
    public string? BusinessPortfolio { get; set; }
    public string? CreditPortfolio { get; set; }
    public string? WfId { get; set; }
    public string? TaskId { get; set; }
    public int? VersionId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? T { get; set; }
    public bool? IsVisible { get; set; }
    public int? Access { get; set; }
}

public class Industrycode
{
    public string? Code { get; set; }
    public string[]? Keys { get; set; }
    public string? Description { get; set; }
}