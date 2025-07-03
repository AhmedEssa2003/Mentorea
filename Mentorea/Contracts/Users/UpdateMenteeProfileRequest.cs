namespace Mentorea.Contracts.Users
{
    public record UpdateMenteeProfileRequest(
        string Name,
        string Location,
        string? About
    );
}
