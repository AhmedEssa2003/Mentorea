using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Authentication
{
    public record MenteeRegisterRequest(
        string Email,
        string Password,
        string Name,
        string Location,
        IFormFile? Image,
        string Gender,
        Date PirthDate,
        string About,
        List<string> FieldInterests
    );
    
}
