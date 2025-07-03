namespace Mentorea.Contracts.Users
{
    public record MenteeProfileResponse(
        string Id,
        string Name,
        string Email,
        string PathPhoto,
        DateOnly PirthDate,
        string Location,
        string? About
    );
}
