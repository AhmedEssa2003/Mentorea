namespace Mentorea.Contracts.Likes
{
    public class LikeRequestValidator : AbstractValidator<LikeRequest>
    {
        public LikeRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
