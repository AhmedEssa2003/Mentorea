namespace Mentorea.Contracts.Users
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Roles)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Roles)
                .Must(r => r.Distinct().Count() == r.Count)
                .WithMessage("Existe Duplicate Role")
                .When(u => u.Roles != null);
        }
    }
}
