namespace Mentorea.Contracts.Posts
{
    public class PostRequestValidator:AbstractValidator<PostRequest>
    {
        public PostRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty();
        }
    }
}
