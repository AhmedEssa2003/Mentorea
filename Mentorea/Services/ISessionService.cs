using Mentorea.Abstractions.Enums;
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Session;

namespace Mentorea.Services
{
    public interface ISessionService
    {
        Task<Result<PaginatedList<SessionResponse>>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken);
        Task<Result<SessionResponse>> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<Result<PaginatedList<SessionResponse>>> GetSessionByUserIdWithStatusAsync(string userId, RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<Result<SessionResponse>> AddAsync(SessionRequest request, string MenteeId, CancellationToken cancellationToken);
        Task<Result> RespondToSessionBeforePayment(string id, ResponseSessionRequest request, CancellationToken cancellationToken);
        Task<Result> ConfirmPaymentAsync(string Id, CancellationToken cancellationToken);
        Task<Result> UpdateSessionAsync(string Id, string userId, UpdateSessionRequest request, CancellationToken cancellationToken);
        Task<Result> RespondToUpdateAsync(string Id, ResponseUpdateReques reques, CancellationToken cancellationToken);
        Task<Result> CancelSessionAsync(string Id,string userId ,CancellationToken cancellationToken);
        Task<Result> GiveFeedbackAsync(string Id, FeedbackRequest request, CancellationToken cancellationToken);
        Task<Result> AttendByOnePartyAsync(string Id, string UserId, CancellationToken cancellationToken);
        Task<Result> SetSessionOutcomeAsync(string Id, SessionOutcomeRequest request, CancellationToken cancellationToken);
        Task<Result> SubmitSessionReportAsync(string Id, CancellationToken cancellationToken);
        Task CancelUnconfirmedSessionsAsync();
    }
}
