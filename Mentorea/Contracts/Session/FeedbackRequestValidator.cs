namespace Mentorea.Contracts.Session
{
    public class FeedbackRequestValidator : AbstractValidator<FeedbackRequest>
    {
        public FeedbackRequestValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty()
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Comment cannot exceed 500 characters.")
                .When(x=>x.Comment is not null);
        }
    }
}
