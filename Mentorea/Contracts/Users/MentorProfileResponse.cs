namespace Mentorea.Contracts.Users
{
    public record MentorProfileResponse(
          string Id,
          string Name,
          string Email,
          string? PathPhoto,
          DateOnly PirthDate,
          string Location,
          decimal? Rate,
          int? PriceOfSession,
          int? NumberOfSession,
          int? NumberOfExperience,
          int? NumerOfComment,
          string? About,
          string? FieldName

    );

}
