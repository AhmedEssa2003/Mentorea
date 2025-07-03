namespace Mentorea.Contracts.TokenDevice
{
    public record SendNotificationRequest(
        string UserId,
        string Title,
        string Body
    );
}
