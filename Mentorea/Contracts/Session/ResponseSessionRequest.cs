using Mentorea.Abstractions.Enums;

namespace Mentorea.Contracts.Session
{
    public record ResponseSessionRequest(
        string Status,
        int? Price
    );
}
