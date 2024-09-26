namespace Data.Repository.POCO.Documents.Allocation;

public class AllocationPayload
{
    public Guid DocumentId { get; set; }
    public int UploadStatus { get; set; }
    public string? Title { get; set; }
    public string? OriginalFileName { get; set; }
    public string? Category { get; set; }
    public string? OperationType { get; set; }

    // ReSharper disable once InconsistentNaming
    public int _uniqueRowId { get; set; }

    // ReSharper disable once InconsistentNaming
    public int BaseVersionId_ { get; set; }

    // ReSharper disable once InconsistentNaming
    public int VersionId_ { get; set; }
}