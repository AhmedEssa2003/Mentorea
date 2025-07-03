namespace Mentorea.Contracts.Users
{
    public record UpdateAdminProfileRequest(
        string Name,
        string Location
    );
}
