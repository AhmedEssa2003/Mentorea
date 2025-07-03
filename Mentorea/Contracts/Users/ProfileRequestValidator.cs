namespace Mentorea.Contracts.Users
{
    public class ProfileRequestValidator : AbstractValidator<ProfileRequest>
    {
        public ProfileRequestValidator()
        {
            RuleFor(x => x.Name);
        }
    }
}
