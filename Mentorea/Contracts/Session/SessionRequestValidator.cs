namespace Mentorea.Contracts.Session
{
    public class SessionRequestValidator:AbstractValidator<SessionRequest>
    {
        public SessionRequestValidator()
        {
            
            RuleFor(x => x.MentorId)
                .NotEmpty();
            RuleFor(x => x.MenteeId)
                .NotEmpty();
            RuleFor(x=>x.DurationMinutes)
                .NotEmpty()
                .GreaterThanOrEqualTo(60)
                .WithMessage("Duration must be greater than 60 minutes.");

            RuleFor(x=>x.WaitingTime)
                .InclusiveBetween(5,10)
                .WithMessage("Waiting time must be greater than or equel 5 minutes and less than or equel 15 minutes.")
                .When(x => x.WaitingTime is not null); 
        }
    }
}
