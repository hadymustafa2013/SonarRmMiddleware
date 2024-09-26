using RmMiddleware.Models.DTO;

namespace RmMiddleware.Models;

public class UserAuthorization
{
    public int? Id { get; set; }
    public object? BusinessPortfolio { get; set; }
    public object? CreditPortfolio { get; set; }
    public string? Role { get; set; }
    public string? UserId { get; set; }
    public List<object>? Inapplicable { get; set; }
    public string? WfId { get; set; }
    public string? TaskId { get; set; }
    public int? VersionId { get; set; }
    public int? BaseVersionId { get; set; }
    public bool? IsLatestVersion { get; set; }
    public bool? IsDeleted { get; set; }
    public int? StatusId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? T { get; set; }
    public string? ContextUserId { get; set; }
    public bool? IsValid { get; set; }
    public bool? IsVisible { get; set; }
    public int? SnapshotId { get; set; }
}

public class User
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? TenantId { get; set; }
    public bool? IsSuperAdministrator { get; set; }
    public int? ConfigId { get; set; }
    public List<UserAuthorization>? UserAuthorization { get; set; }
}

public class PayLoad
{
    public User? User { get; set; }
    public string? Token { get; set; }
    public bool? DevMode { get; set; }
}

public class AuthResponse
{
    public PayLoad? PayLoad { get; set; }
    public Status? Status { get; set; }
}

public class Warn
{
    public string? ResourceId { get; set; }
    public bool? ShowMessage { get; set; }
}