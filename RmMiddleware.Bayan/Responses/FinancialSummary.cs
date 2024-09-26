using Newtonsoft.Json.Linq;

namespace RmMiddleware.Bayan.Responses;

public class FinancialSummary
{
    public JObject? Payload { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public string? CC { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? CC_DESC { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? DAT { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? BID { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? FID { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? TYPE { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? AMN { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? AO { get; set; }

    // ReSharper disable once InconsistentNaming
    public bool? IA { get; set; }

    // ReSharper disable once InconsistentNaming
    public bool? CS { get; set; }

    // ReSharper disable once InconsistentNaming
    public string? VER { get; set; }

    // ReSharper disable once InconsistentNaming
    public long REV { get; set; }

    // ReSharper disable once InconsistentNaming
    public long NET_INC { get; set; }

    // ReSharper disable once InconsistentNaming
    public long TOT_ASSET { get; set; }

    // ReSharper disable once InconsistentNaming
    public long TOT_LIAB { get; set; }

    // ReSharper disable once InconsistentNaming
    public List<FIT>? FIT { get; set; }
    
    // ReSharper disable once InconsistentNaming
    public string? PDF { get; set; }
    // ReSharper disable once InconsistentNaming
    public string? FN { get; set; }
}