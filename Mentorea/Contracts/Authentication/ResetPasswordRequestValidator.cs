using Mentorea.Abstractions.Consts;

namespace Mentorea.Contracts.Authentication
{
    public class ResetPasswordRequestValidator:AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Code)
                .NotEmpty();
            RuleFor(x => x.NewPassword)
                .Matches(RegexPatterns.Password)
                .WithMessage("Password should be at least 8 digits and should be lowercase,uppercase and non alphanumeric ");
        }
    }
}
