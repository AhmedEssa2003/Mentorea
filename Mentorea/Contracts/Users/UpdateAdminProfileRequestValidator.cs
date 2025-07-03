namespace Mentorea.Contracts.Users
{
    public class UpdateAdminProfileRequestValidator : AbstractValidator<UpdateAdminProfileRequest>
    {

        public UpdateAdminProfileRequestValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 150);

            

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(300);
        }
    }
}
