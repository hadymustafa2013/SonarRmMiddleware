using Data.Repository.CreditLens.Attributes;

namespace Data.Repository.POCO;

public class BayanSettings
{
    [PrimaryKey("Sequence")]
    public long Id { get; set; }
    public string? FinancialTemplate { get; set; } 
    public string? BayanHttpEndpointJwt { get; set; }
    public string? BayanHttpEndpointReport { get; set; }
    public string? BayanHttpEndpointStatementList { get; set; }
    public string? BayanHttpUser { get; set; }
    [Encrypt]
    public string? BayanHttpPassword { get; set; }
    public string? BayanDocumentUploadLocation { get; set; }
    public int OrigRounding { get; set; }
    [Ignore]
    public bool BayanAuthentication { get; set; }
    [Ignore]
    public bool BayanChannelIdHeader { get; set; }
}