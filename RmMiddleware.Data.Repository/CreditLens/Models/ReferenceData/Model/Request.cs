namespace Data.Repository.CreditLens.Models.ReferenceData.Model;

public class Request
{
    public List<Attribute>? Attribute { get; set; } 
    public string? Id { get; set; } 
    public string? Source { get; set; }
    public bool VersionEnable { get; set; }
    public bool Hierarchy { get; set; }
    public string? DataProvider { get; set; }
    public string? ModelCategory { get; set; }
    public bool NotesEnabled { get; set; }
    public bool MultiModule { get; set; }
    public bool DisplayAsIcon { get; set; }
    public bool ShowAsPill { get; set; }
}