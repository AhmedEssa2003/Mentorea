using Mentorea.Contracts.Comments;
using Mentorea.Contracts.Common;
using Mentorea.Errors;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class CommentService(MentoreaDbContext context) : ICommentService
    {
        private readonly MentoreaDbContext _context = context;
        public async Task<Result<PaginatedList<CommentResponse>>> GetAllAsync(RequestFilters requestFilters, string postId,CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(p => p.Id == postId, cancellationToken))
                return Result.Failure<PaginatedList<CommentResponse>>(PostError.NotFound);

            var query = _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .Select(x=>new CommentResponse(x.Id,x.Content,x.PostId,x.UserId,x.CreatedAt,x.User.PathPhoto,x.User.Name));

            var paginatedResponse = await PaginatedList<CommentResponse>.CreateAsync(
                query.AsNoTracking(),
                requestFilters.PageNumber,
                requestFilters.PageSize,
                cancellationToken
            );

            return Result.Success(paginatedResponse);

        }
        public async Task<Result<CommentResponse>> GetAsync(string postId,string CommentId, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(p => p.Id == postId, cancellationToken))
                return Result.Failure<CommentResponse>(PostError.NotFound);
            if(await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c=>c.Id == CommentId && c.PostId == postId, cancellationToken)
                is not { } Comment)
                return Result.Failure<CommentResponse>(CommentError.NotFound);
            var comment = new CommentResponse(Comment.Id, Comment.Content, Comment.PostId, Comment.UserId, Comment.CreatedAt, Comment.User.PathPhoto, Comment.User.Name);
            return Result.Success(comment);
        }
        public async Task<Result<CommentResponse>> CreateAsync(string postId, string UserId, CommentRequest request, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(p => p.Id == postId, cancellationToken))
                return Result.Failure<CommentResponse>(PostError.NotFound);
            var comment = request.Adapt<Comment>();
            comment.UserId = UserId;
            comment.PostId = postId;
            comment.Id = Guid.NewGuid().ToString();
            await _context.Comments.AddAsync(comment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            comment = await _context.Comments
                .Include(c => c.User)
                .SingleOrDefaultAsync(c => c.Id == comment.Id && c.PostId == postId, cancellationToken);
            var Comment = new CommentResponse(comment!.Id, comment.Content, comment.PostId, comment.UserId, comment.CreatedAt, comment.User.PathPhoto, comment.User.Name);
            return Result.Success(Comment);
        }
        public async Task<Result> UpdateAsync(string postId, string CommentId,string userId, CommentRequest request, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(p => p.Id == postId, cancellationToken))
                return Result.Failure<CommentResponse>(PostError.NotFound);
            if (await _context.Comments.SingleOrDefaultAsync(c => c.Id == CommentId && c.PostId == postId, cancellationToken) is not { } comment)
                return Result.Failure(CommentError.NotFound);
            if (comment.UserId != userId)
                return Result.Failure(CommentError.Unauthorized);
            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow.AddHours(3);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> DeleteAsync(string postId, string CommentId, CancellationToken cancellationToken)
        {
            if (!await _context.Posts.AnyAsync(p => p.Id == postId, cancellationToken))
                return Result.Failure<CommentResponse>(PostError.NotFound);
            if (await _context.Comments.SingleOrDefaultAsync(c => c.Id == CommentId && c.PostId == postId, cancellationToken) is not { } comment)
                return Result.Failure(CommentError.NotFound);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
