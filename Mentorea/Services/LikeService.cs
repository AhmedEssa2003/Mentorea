using Mentorea.Contracts.Common;
using Mentorea.Contracts.Follows;
using Mentorea.Contracts.Likes;
using Mentorea.Errors;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class LikeService(MentoreaDbContext context) : ILikeService
    {
        private readonly MentoreaDbContext _context = context;
        public async Task<Result> ToggleLikeAsync(string PostId, LikeRequest request, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(x => x.Id == PostId, cancellationToken))
                return Result.Failure(PostError.NotFound);

            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
                return Result.Failure(UserError.NotFoundUser);

            if (await _context.Posts.AnyAsync(x => x.Id == PostId && x.UserId == request.UserId, cancellationToken))
                return Result.Failure(LikeError.InvalidOperation);

            var like = await _context.Likes
                .Where(x => x.PostId == PostId && x.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (like != null)
                _context.Likes.Remove(like);
            
            else
                _context.Likes.Add(new Like { PostId = PostId, UserId = request.UserId });
            
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result<int>> GetLikesCountAsync(string PostId, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(x => x.Id == PostId, cancellationToken))
                return Result.Failure<int>(PostError.NotFound);
            var count = await _context.Likes
                .Where(x => x.PostId == PostId)
                .CountAsync(cancellationToken);
            return Result.Success(count);
        }
        public async Task<Result<PaginatedList<LikeResponse>>> GetLikesAsync(string postId, RequestFilters request, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(x => x.Id == postId, cancellationToken))
                return Result.Failure<PaginatedList<LikeResponse>>(PostError.NotFound);

            var likesQuery = _context.Likes
                .Where(x => x.PostId == postId )
                .Include(x => x.User)
                .OrderBy($"CreatedAt {request.SortDirection}")
                .Select(x => new LikeResponse(x.UserId,x.User.Name,x.User.PathPhoto));

            var response = await PaginatedList<LikeResponse>
                .CreateAsync(likesQuery.AsNoTracking(), request.PageNumber, request.PageSize, cancellationToken);

            return Result.Success(response);
        }

    }
}
