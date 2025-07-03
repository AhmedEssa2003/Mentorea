namespace Mentorea.Contracts.Specializations
{
    public class SpecializationRequestValidator:AbstractValidator<SpecializationRequest>
    {
        public SpecializationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");
        }
    }
    
}
