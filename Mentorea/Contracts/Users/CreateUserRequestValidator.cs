namespace Mentorea.Contracts.Users
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        private readonly string[] _allowedGender = { "Male", "Female" };
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should be lowercase,uppercase and non alphanumeric ");

            RuleFor(x => x.Role)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(x => _allowedGender.Contains(x));

            RuleFor(x => x.PirthDate)
                .NotEmpty()
                .Must(x => DateTime.UtcNow.AddHours(3).Year - x.Year >= 6);

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(300);

            
        }
    }
}
