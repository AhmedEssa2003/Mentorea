using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public class SessionOutcomeRequestValidator: AbstractValidator<SessionOutcomeRequest>
    {
        private readonly string[] _validStatuses = Enum.GetNames(typeof(ApprovalStatus));
        public SessionOutcomeRequestValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(x => _validStatuses.Contains(x))
                .WithMessage("Status must be a valid enum value.");
        }
    }
    
}
