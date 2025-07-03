namespace Mentorea.Contracts.Users
{
    public class UpdateMenteeProfileRequestValidator:AbstractValidator<UpdateMenteeProfileRequest>
    {

        public UpdateMenteeProfileRequestValidator()
        {

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 150);

            

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.About)
                .MaximumLength(400);
        }
    }
}
