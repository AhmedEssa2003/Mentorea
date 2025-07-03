namespace Mentorea.Contracts.Users
{
    public record UpdateMentorProfileRequest(
        string Name,
        string Location,
        int? PriceOfSession,
        string? About
    );
}
