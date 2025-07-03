using Mentorea.Abstractions.Consts;

namespace Mentorea.Contracts.Users
{
    public class UpdateMentorProfileRequestValidator:AbstractValidator<UpdateMentorProfileRequest>
    {

        public UpdateMentorProfileRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 150);

            RuleFor(x => x.PriceOfSession)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(700)
                .When(x=>x.PriceOfSession is not null);

            

            RuleFor(x => x.Location)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.About)
                .MaximumLength(400);
        }
    }
}
