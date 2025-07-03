using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public class ResponseSessionRequestValidator:AbstractValidator<ResponseSessionRequest>
    {
        private readonly string[] _validStatuses = Enum.GetNames(typeof(ApprovalStatus));
        public ResponseSessionRequestValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(x=>_validStatuses.Contains(x))
                .WithMessage("Status must be either accepted or rejected");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(700)
                .When(x=>x.Price is not null);

        }
    }
    
}
