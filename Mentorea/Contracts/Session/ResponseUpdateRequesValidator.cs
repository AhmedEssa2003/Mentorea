using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public class ResponseUpdateRequesValidator:AbstractValidator<ResponseUpdateReques>
    {
        private readonly string[] _validStatuses = Enum.GetNames(typeof(ApprovalStatus));
        public ResponseUpdateRequesValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty()
                .Must(x => _validStatuses.Contains(x))
                .WithMessage("Status must be a valid enum value.");
        }
    }
    
}
