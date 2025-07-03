using Mentorea.Abstractions.Consts;

namespace Mentorea.Contracts.Users
{
    public class ChangePasswordRequestValidator:AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password);
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should be lowercase,uppercase and non alphanumeric ")
                .NotEqual(x => x.CurrentPassword);

        }
    }
}
