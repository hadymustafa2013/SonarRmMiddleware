using Newtonsoft.Json;

namespace RmMiddleware.CreditLensAggregator.Dto.Spreading.SpreadingResponse;

public class Entity
{
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? Id { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? Cif { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ProjectionId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ProjectionName { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public DateTime? ProjectionModifiedDate { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? FinancialId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? FinancialTemplateId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ApprovedFinalGrade { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ApprovedFinalPd { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ApprovedModelGrade {get;set;}
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ApprovedModelPd { get; set; } 
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ApprovedOverrideGrade { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ApprovedOverridePd { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ApprovedOverrideReason { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ApprovedOverrideComments { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ApprovedModelId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public DateTime? ApprovedDate { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? CustomerName { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ProposedFinalGrade { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ProposedFinalPd { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? IndustryClassification { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? IndustryCode { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? PeerClassification { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? PeerCode { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ProposedModelGrade { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ProposedModelPd { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public int? ProposedOverrideGrade { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public double? ProposedOverridePd { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ProposedOverrideReason { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ProposedOverrideComments { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? ProposedModelId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public RatingValue? RatingValues { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Statement> Statements { get; set; } = [];
}