using Mentorea.Abstractions.Consts;

namespace Mentorea.Contracts.Authentication
{
    public class MenteeRegisterRequestValidator:AbstractValidator<MenteeRegisterRequest>
    {
        private readonly string[] _allowedExtensions = {".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedGender = { "Male", "Female" };
        public MenteeRegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .MaximumLength(100)
                .EmailAddress()
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 150);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.About)
                .MaximumLength(400);

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(x => _allowedGender.Contains(x));

            RuleFor(x => x.PirthDate)
                .NotEmpty()
                .Must(x => DateTime.UtcNow.AddHours(3).Year - x.Year >= 6);

            RuleFor(x => x.FieldInterests)
                .NotEmpty()
                .Must(x=>x.Distinct().Count()==x.Count);

            RuleFor(x => x.Password)
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should be lowercase,uppercase and non alphanumeric ");

            RuleFor(x => x.Image)
                .Must(x => _allowedExtensions.Contains(Path.GetExtension(x!.FileName)))
                .WithMessage("File extension is allowed { \".jpg\", \".jpeg\", \".png\", \".webp\" } only ")
                .Must(x => x!.Length < 1 * 1024 * 1024)
                .WithMessage("File size should be less than 1MB")
                .When(x => x.Image != null);

        }
    }
}
