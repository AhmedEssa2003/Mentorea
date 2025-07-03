namespace Mentorea.Contracts.Follows
{
    public record FollowResponse(
        string UserId,
        DateOnly CreatedAt,
        string? PathImage,
        string Name
    );
}
