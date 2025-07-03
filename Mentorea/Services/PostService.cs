using Hangfire;
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Posts;
using Mentorea.Errors;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class PostService(MentoreaDbContext context,
        IFcmService fcmService
        ) : IPostService
    {
        private readonly MentoreaDbContext _context = context;
        private readonly IFcmService _fcmService = fcmService;

        public async Task<PaginatedList<PostResponse>> GetAllSync(RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            var query = _context.Posts
                .Where(x=>(string.IsNullOrEmpty(requestFilters.SearchValue) 
                    || x.UserId == requestFilters.SearchValue))
                .Include(x => x.User)
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .Select(x => new PostResponse(
                    x.Id,
                    x.Content,
                    x.UserId,
                    x.CreatedAt,
                    x.User.PathPhoto,
                    x.User.Name,
                    _context.Comments.Count(c => c.PostId == x.Id),
                    _context.Likes.Count(l => l.PostId == x.Id)
                ))
                .AsNoTracking();
            
            var response = await PaginatedList<PostResponse>
                .CreateAsync(query, requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);
            return response;
        }
        public async Task<Result<PostResponse>> GetSync(string id, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Include(x => x.User) 
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (post is null)
                return Result.Failure<PostResponse>(PostError.NotFound);

            var countComment = await _context.Comments.Where(x => x.PostId == post.Id).CountAsync(cancellationToken);
            var countLike = await _context.Likes.Where(x => x.PostId == post.Id).CountAsync(cancellationToken);

            var postResponse = new PostResponse(
                post.Id,
                post.Content,
                post.UserId,
                post.CreatedAt,
                post.User.PathPhoto,
                post.User.Name,
                countComment,
                countLike
            );

            return Result.Success(postResponse);


        }
        public async Task<Result<PaginatedList<PostResponse>>> GetFollowedPostsAsync(string userId, RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken))
                return Result.Failure<PaginatedList<PostResponse>>(UserError.NotFoundUser);

            var query = _context.Posts
                .Where(p => _context.Follows
                    .Any(x => x.FollowerId == userId && x.FollowedId == p.UserId)
                    &&(string.IsNullOrEmpty(requestFilters.SearchValue) ||p.Content.Contains(requestFilters.SearchValue) ))
                .Include(x => x.User)
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .Select(x => new PostResponse(
                    x.Id,
                    x.Content,
                    x.UserId,
                    x.CreatedAt,
                    x.User.PathPhoto,
                    x.User.Name,
                    _context.Comments.Count(c => c.PostId == x.Id),
                    _context.Likes.Count(l => l.PostId == x.Id)
                ));
            var posts = await PaginatedList<PostResponse>.CreateAsync(query.AsNoTracking(), requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);

            return Result.Success(posts);
        }

        public async Task<Result<PostResponse>> CreateSync(string UserId,PostRequest request, CancellationToken cancellationToken)
        {
            var post = request.Adapt<Post>();
            post.UserId = UserId;
            post.Id = Guid.NewGuid().ToString();
            await _context.Posts.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == post.Id, cancellationToken);
            var followers = await _context.Follows
                .Where(x => x.FollowedId == UserId)
                .Select(x => x.FollowerId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            foreach (var follower in followers)
                BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                    follower,
                    "New Post from Your Mentor",
                    "A mentor you follow has shared a new post. Open the app to check it out."
                    , cancellationToken
                ));
            var CountComment = await _context.Comments.Where(x => x.PostId == post!.Id).CountAsync(cancellationToken);
            var CountLike = await _context.Likes.Where(x => x.PostId == post!.Id).CountAsync(cancellationToken);
            var postResponse = new PostResponse(post!.Id, post.Content, post.UserId, post.CreatedAt, post.User.PathPhoto, post.User.Name,CountComment,CountLike);
            return Result.Success(postResponse);
        }
        public async Task<Result> UpdateSync(string id,string userId, PostRequest request, CancellationToken cancellationToken)
        {
            if (await _context.Posts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is not { } post)
                return Result.Failure(PostError.NotFound);
            if (post.UserId != userId)
                return Result.Failure(PostError.Unauthorized);
            post.Content = request.Content;
            post.UpdatedAt = DateTime.UtcNow.AddHours(3);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        public async Task<Result> DeleteSync(string id, CancellationToken cancellationToken)
        {
            if (await _context.Posts.SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is not { } post)
                return Result.Failure(PostError.NotFound);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
