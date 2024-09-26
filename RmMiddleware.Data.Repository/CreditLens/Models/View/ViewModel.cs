namespace Data.Repository.CreditLens.Models.View;

public class ViewModel
{
    // ReSharper disable once CollectionNeverQueried.Global
    public List<Group>? Group { get; set; }
    public bool GroupSpecified { get; set; }
    public bool ToolbarButtonSpecified { get; set; }
    public bool SearchQuerySpecified { get; set; }
    public bool ConditionGroupSpecified { get; set; }
    public string? Id { get; set; }
    public string? PrimaryModel { get; set; }
    public string? Type { get; set; }
    public int ColSpan { get; set; }
    public string? QueryFilterChainId { get; set; }
    public string? AccessControlChainId { get; set; }
    public int MaxSearchResultCount { get; set; }
    public int PageSize { get; set; }
    public bool IsInfiniteScroll { get; set; }
    public bool IsSearch { get; set; }
    public string? ViewModelCategory { get; set; }
    public string? Description { get; set; }
    public string? ResourceModuleId { get; set; }
    public bool NotesEnabled { get; set; }
    public bool Shared { get; set; }
    public bool AllowDraft { get; set; }
    public bool UseBehavior { get; set; }
}

