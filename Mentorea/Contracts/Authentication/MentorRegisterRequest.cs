using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Authentication
{
    public record MentorRegisterRequest(
        string Email,
        string Password,
        string Name,
        string Location,
        IFormFile? Image,
        string Gender,
        Date PirthDate,
        int NumberOfExperience,
        int? PriceOfSession,
        string About,
        string FieldId
    );
}
