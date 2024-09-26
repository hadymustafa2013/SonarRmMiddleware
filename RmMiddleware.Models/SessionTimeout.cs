namespace RmMiddleware.Models;

public class SessionTimeout
{
    public DateTime LoginTime { get; set; }
    public DateTime? ExpiryTime => LoginTime.AddMilliseconds(ExpiresMs);
    public string? Duration { get; set; }
    public string? Expires { get; set; }
    public int ExpiresMs { get; set; }
}