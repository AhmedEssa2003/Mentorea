namespace Mentorea.Contracts.Likes
{
    public record LikeResponse(
        string UserId,
        string Name,
        string? PathImage
    );
}
