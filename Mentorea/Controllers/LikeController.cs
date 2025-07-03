using Mentorea.Contracts.Common;
using Mentorea.Contracts.Likes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/Posts/{postId}/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
    public class LikeController(ILikeService likeService) : ControllerBase
    {
        private readonly ILikeService _likeService = likeService;
        [HttpPut("")]
        public async Task<IActionResult> ToggleLike([FromRoute]string postId,[FromBody]LikeRequest request, CancellationToken cancellationToken)
        {
            var result = await _likeService.ToggleLikeAsync(postId, request, cancellationToken);
            return result.IsSuccess ? NoContent() :result.ToProblem();
        }
        [HttpGet("Count")]
        public async Task<IActionResult> GetLikesCount([FromRoute] string postId, CancellationToken cancellationToken)
        {
            var result = await _likeService.GetLikesCountAsync(postId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpGet("user")]
        public async Task<IActionResult> GetLikesUsers([FromRoute] string postId,[FromQuery]RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await _likeService.GetLikesAsync(postId,request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
    }
}
