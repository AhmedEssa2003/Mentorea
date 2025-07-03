namespace Mentorea.Contracts.Comments
{
    public record CommentResponse(
        string Id,
        string Content,
        string PostId,
        string UserId,
        DateTime CreatedAt,
        string? PathImage,
        string Name
    );
}
