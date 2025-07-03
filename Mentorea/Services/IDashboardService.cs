using Mentorea.Contracts.Dashboard;

namespace Mentorea.Services
{
    public interface IDashboardService
    {
        Task<int> CountOfMentorAsync(CancellationToken cancellationToken);
        Task<int> CountOfMenteeAsync(CancellationToken cancellationToken);
        Task<int> CountOfPostAsync(CancellationToken cancellationToken);
        Task<int> CountOfSessionAsync(CancellationToken cancellationToken);
        Task<Result<List<DashboardSessionResponse>>> GetPaymentRecipientAsync(CancellationToken cancellationToken);
        Task<Result> DeleteFromListAsync(string SessionId, CancellationToken cancellationToken);
    }
}
