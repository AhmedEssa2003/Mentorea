using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Users
{
    public record CreateUserRequest(
        string Email,
        string Password,
        string Name,
        string Gender,
        string Location,
        Date PirthDate,
        string Role
    );
}
