namespace Mentorea.Contracts.Chat
{
    public record MessageRequest
    (
        string receiverId,
        string content,
        string type
    );
}
