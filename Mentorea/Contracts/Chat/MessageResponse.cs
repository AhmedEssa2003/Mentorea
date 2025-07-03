namespace Mentorea.Contracts.Chat
{
    public record MessageResponse(
        string Id,
        string SenderId,
        string ReceiverId,
        string Content,
        string Type,
        DateTime CreatedAt
    );
}
