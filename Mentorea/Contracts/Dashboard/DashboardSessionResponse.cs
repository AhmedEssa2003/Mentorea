namespace Mentorea.Contracts.Dashboard
{
    public record DashboardSessionResponse(
        string Id,
        string UserId,
        string UserName,
        string? CardId,
        string Status,
        int? Price
    );
}
