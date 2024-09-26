namespace RmMiddleware.ABOControllers;

public class EntityRatingInfo
{
    public long? EntityId { get; set; }
    public string? Cif { get; set; }
    public string? CustomerName { get; set; }
    public RatingInfo? RatingInfo { get; set; }
    public EntityLgdInfo? ProposedEntityLgdInfo { get; set; }
}