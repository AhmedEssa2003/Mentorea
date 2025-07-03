namespace Mentorea.Contracts.Comments
{
    public class CommentRequestValibator : AbstractValidator<CommentRequest>
    {
        public CommentRequestValibator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required");
        }
    }
}
