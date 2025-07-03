namespace Mentorea.Contracts.Users
{
    public record UpdateUserRequest(
        string Email,
        string Name,
        IList<string> Roles
    );

}
