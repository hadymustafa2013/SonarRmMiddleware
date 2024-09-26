namespace Data.Repository.POCO.Documents.Document;

public class DocumentPayload
{
    public Guid DocumentId { get; set; }

    // ReSharper disable once InconsistentNaming
    public int VersionId_ { get; set; }

    // ReSharper disable once InconsistentNaming
    public int BaseVersionId_ { get; set; }
    public string? OriginalFileName { get; set; }
}