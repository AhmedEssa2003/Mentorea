
namespace Mentorea.Contracts.Authentication
{
    public class ConfirmEmailRequestValidator:AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .Length(6);
            RuleFor(x => x.Email)
               .NotEmpty();
        }
    }
}
