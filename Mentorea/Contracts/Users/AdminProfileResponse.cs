namespace Mentorea.Contracts.Users
{
    public record AdminProfileResponse(
        string Id,
        string Name,
        string Email,
        string PathPhoto,
        DateOnly PirthDate,
        string Location
    );
}
