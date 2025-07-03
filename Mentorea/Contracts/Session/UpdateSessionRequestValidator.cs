namespace Mentorea.Contracts.Session
{
    public class UpdateSessionRequestValidator:AbstractValidator<UpdateSessionRequest>
    {
        public UpdateSessionRequestValidator()
        {
            RuleFor(x => x.ScheduledTime)
                .NotEmpty()
                .NotNull()
                .Must(x => DateTime.UtcNow.AddHours(3).AddMinutes(10) < x)
                .WithMessage("The value must be greater than the current time plus 10 minutes.");
            RuleFor(x => x.DurationMinutes)
                .NotEmpty()
                .GreaterThanOrEqualTo(20)
                .WithMessage("Duration must be greater than 20 minutes.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(700)
                .When(x => x.Price is not null);
            RuleFor(x => x.WaitingTime)
                .InclusiveBetween(5, 10)
                .WithMessage("Waiting time must be greater than or equal to 5 minutes and less than or equal to 10 minutes.")
                .When(x => x.WaitingTime is not null);
        }
    }
    
}
