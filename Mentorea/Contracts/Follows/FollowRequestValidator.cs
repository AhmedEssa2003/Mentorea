namespace Mentorea.Contracts.Follows
{
    public class FollowRequestValidator : AbstractValidator<FollowRequest>
    {
        public FollowRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
