namespace Mentorea.Contracts.Users
{
    public record UserResponse(
        string Id,
        string Name,
        string Email,
        string Roles
    );

}
