using Mentorea.Contracts.Common;
using Mentorea.Contracts.Follows;

namespace Mentorea.Services
{
    public interface IFollowService
    {
        Task<Result> ToggleFollowAsync(string FollowerId, FollowRequest request, CancellationToken cancellationToken);
        Task<Result<PaginatedList<FollowResponse>>> GetFollowedsAsync(FollowRequest request,RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<Result<PaginatedList<FollowResponse>>> GetFollowersAsync(FollowRequest request,RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<Result<int>> GetCountFollowedsAsync(string userId, CancellationToken cancellationToken);
        Task<Result<int>> GetCountFollowersAsync(string userId, CancellationToken cancellationToken);
    }
}
