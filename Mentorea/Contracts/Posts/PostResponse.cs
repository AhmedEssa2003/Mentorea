namespace Mentorea.Contracts.Posts
{
    public record PostResponse(
        string Id,
        string Content,
        string UserId,
        DateTime CreatedAt,
        string? PathImage,
        string Name,
        int CountComment,
        int CountLike
    );
}
