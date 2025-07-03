using Mentorea.Contracts.Common;
using Mentorea.Contracts.Follows;


namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class FollowController(IFollowService followService) : ControllerBase
    {
        private readonly IFollowService _followService = followService;
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPut("")]
        public async Task<IActionResult> ToggleFollow([FromBody] FollowRequest request, CancellationToken cancellationToken)
        {
            var result = await _followService.ToggleFollowAsync(User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("Followed")]
        public async Task<IActionResult> GetFolloweds([FromBody] FollowRequest request,[FromQuery]RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            var result = await _followService.GetFollowedsAsync(request,requestFilters, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("Follower")]
        public async Task<IActionResult> GetFollowers([FromBody] FollowRequest request,[FromQuery]RequestFilters requestFilters,CancellationToken cancellationToken)
        {
            var result = await _followService.GetFollowersAsync(request,requestFilters, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("{userId}/followers/count")]
        public async Task<IActionResult> GetFollowersCount(string userId, CancellationToken cancellationToken)
        {
            var result = await _followService.GetCountFollowersAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("{userId}/followings/count")]
        public async Task<IActionResult> GetFollowingsCount(string userId, CancellationToken cancellationToken)
        {
            var result = await _followService.GetCountFollowedsAsync(userId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
    }
}
