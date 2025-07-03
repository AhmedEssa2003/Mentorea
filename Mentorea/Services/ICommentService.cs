using Mentorea.Contracts.Comments;
using Mentorea.Contracts.Common;

namespace Mentorea.Services
{
    public interface ICommentService
    {
        Task<Result<PaginatedList<CommentResponse>>> GetAllAsync(RequestFilters requestFilters, string postId, CancellationToken cancellationToken);
        Task<Result<CommentResponse>> GetAsync(string postId, string CommentId, CancellationToken cancellationToken);
        Task<Result<CommentResponse>> CreateAsync(string postId, string UserId, CommentRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(string postId, string CommentId,string userId, CommentRequest request, CancellationToken cancellationToken);
        Task<Result> DeleteAsync(string postId, string CommentId, CancellationToken cancellationToken);
    }
}
