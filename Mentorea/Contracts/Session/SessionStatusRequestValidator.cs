using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public class SessionStatusRequestValidator : AbstractValidator<SessionStatusRequest>
    {
        private readonly string[] _status = Enum.GetNames(typeof(SessionStatus));
        public SessionStatusRequestValidator()
        {
            RuleFor(x => x.status)
                .Must(x=> _status.Contains(x));
        }
    }

}
