namespace Mentorea.Contracts.MentorAvailability
{
    public class MentorAvailabilityRequestValidator:AbstractValidator<MentorAvailabilityRequest>
    {
        public MentorAvailabilityRequestValidator()
        {
            
            RuleFor(x => x.StartDateTime)
                .NotEmpty()
                .NotNull()
                .WithMessage("StartDate is required.")
                .Must(x => DateTime.UtcNow.AddHours(3) <= x)
                .When(x=> x.StartDateTime.HasValue)
                .WithMessage("StartDate must be a valid date.");
            RuleFor(x => x.EndDateTime)
                .NotEmpty()
                .NotNull()
                .WithMessage("EndDate is required.")
                .Must(x => DateTime.UtcNow.AddHours(3) <= x)
                .When(x=>x.EndDateTime.HasValue)
                .WithMessage("EndDate must be a valid date.")
                .GreaterThan(x => x.StartDateTime)
                .When(x=>x.StartDateTime.HasValue && x.EndDateTime.HasValue)
                .WithMessage("EndDate must be greater than StartDate.");
        }
        
    }
    
}
