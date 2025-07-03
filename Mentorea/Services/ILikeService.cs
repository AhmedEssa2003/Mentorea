using Mentorea.Contracts.Common;
using Mentorea.Contracts.Likes;

namespace Mentorea.Services
{
    public interface ILikeService
    {
        Task<Result> ToggleLikeAsync(string PostId, LikeRequest request, CancellationToken cancellationToken);
        Task<Result<int>> GetLikesCountAsync(string PostId, CancellationToken cancellationToken);
        Task<Result<PaginatedList<LikeResponse>>> GetLikesAsync(string postId, RequestFilters request, CancellationToken cancellationToken);
    }
}
