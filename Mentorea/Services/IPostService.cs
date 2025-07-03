using Mentorea.Contracts.Common;
using Mentorea.Contracts.Posts;

namespace Mentorea.Services
{
    public interface IPostService
    {
        Task<PaginatedList<PostResponse>> GetAllSync(RequestFilters requestFilters, CancellationToken cancellationToken);
        Task<Result<PostResponse>> GetSync(string id, CancellationToken cancellationToken);
        Task<Result<PostResponse>> CreateSync(string UserId, PostRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateSync(string id,string userId, PostRequest request, CancellationToken cancellationToken);
        Task<Result> DeleteSync(string id, CancellationToken cancellationToken);
        Task<Result<PaginatedList<PostResponse>>> GetFollowedPostsAsync(string userId, RequestFilters requestFilters, CancellationToken cancellationToken);
    }
}
