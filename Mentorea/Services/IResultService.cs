using Mentorea.Contracts.Common;
using Mentorea.Contracts.Users;

namespace Mentorea.Services
{
    public interface IResultService
    {
        Task<PaginatedList<MentorProfileResponse>> GetAllMentorsAsync(RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<PaginatedList<MentorProfileResponse>> MentorsBySpecializationAsync(RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<Result<PaginatedList<MentorProfileResponse>>> GetRecomendedMentors(string userId, RequestFilters requestFilters, CancellationToken cancellationToken);
    }
}
