using Hangfire;
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Follows;
using Mentorea.Errors;
using System.Linq.Dynamic.Core;

namespace Mentorea.Services
{
    public class FollowService(MentoreaDbContext context,
        IFcmService fcmService
        ) : IFollowService
    {
        private readonly MentoreaDbContext _context = context;
        private readonly IFcmService _fcmService = fcmService;

        public async Task<Result> ToggleFollowAsync(string FollowerId, FollowRequest request, CancellationToken cancellationToken)
        {
            if (FollowerId == request.UserId )
                return Result.Failure(FollowErorr.InvalidOperation);

            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
                return Result.Failure(FollowErorr.NotFoundFollowed);

           if (await _context.Users.AnyAsync(x=>x.Id == request.UserId && x.NumberOfExperience ==null, cancellationToken))
                return Result.Failure(FollowErorr.InvalidOperation);

            if (_context.Follows.Any(x => x.FollowerId == FollowerId && x.FollowedId == request.UserId))
                 _context.Follows.Remove(new Follow { FollowerId = FollowerId, FollowedId = request.UserId });
            
            else
                 _context.Follows.Add(new Follow { FollowerId = FollowerId, FollowedId = request.UserId });
            await _context.SaveChangesAsync(cancellationToken);
            BackgroundJob.Enqueue(() => _fcmService.SendPushNotificationToUserDevicesAsync(
                request.UserId,
                "New Follower Alert",
                "Someone has started following you. Open the app to view their profile.",
                cancellationToken
            ));
            return Result.Success();
        }
        //mentee
        public async Task<Result<int>>GetCountFollowersAsync(string userId, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken))
                return Result.Failure<int>(UserError.NotFoundUser);
            if (await _context.Users.AnyAsync(x => x.Id == userId && x.NumberOfExperience == null, cancellationToken))
                return Result.Failure<int>(FollowErorr.InvalidOperation);
            var count = await _context.Follows.CountAsync(x => x.FollowedId == userId, cancellationToken);
            return Result.Success(count);
        }
        public async Task<Result<int>> GetCountFollowedsAsync(string userId, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken))
                return Result.Failure<int>(UserError.NotFoundUser);
            var count = await _context.Follows.CountAsync(x => x.FollowerId == userId, cancellationToken);
            return Result.Success(count);
        }
        public async Task<Result<PaginatedList<FollowResponse>>> GetFollowedsAsync(FollowRequest request,RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
                return Result.Failure<PaginatedList<FollowResponse>>(UserError.NotFoundUser);

            var followedsQuery = _context.Follows
                .Where(x => x.FollowerId == request.UserId)
                .Include(x => x.Followed)
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .Select(x => new FollowResponse(x.FollowedId, x.CreatedAt,x.Followed.PathPhoto,x.Followed.Name));

            var response = await PaginatedList<FollowResponse>
                .CreateAsync(followedsQuery.AsNoTracking(), requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);

            return Result.Success(response);
        }
        //Mentor
        public async Task<Result<PaginatedList<FollowResponse>>> GetFollowersAsync(FollowRequest request,RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            if (!await _context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken))
                return Result.Failure<PaginatedList<FollowResponse>>(UserError.NotFoundUser);

            var followersQuery = _context.Follows
                .Where(x => x.FollowedId == request.UserId)
                .Include(x => x.Follower)
                .OrderBy($"CreatedAt {requestFilters.SortDirection}")
                .Select(x => new FollowResponse(x.FollowerId, x.CreatedAt,x.Follower.PathPhoto,x.Follower.Name));

            var response = await PaginatedList<FollowResponse>
                .CreateAsync(followersQuery.AsNoTracking(), requestFilters.PageNumber, requestFilters.PageSize, cancellationToken);

            return Result.Success(response);
        }
    }
}
