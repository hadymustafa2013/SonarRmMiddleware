using RmMiddleware.Models.DTO;

namespace RmMiddleware.Models;

public class ClResponse
{
    public List<Dictionary<string, object>>? PayLoad { get; set; }
    public Status? Status { get; set; }
}