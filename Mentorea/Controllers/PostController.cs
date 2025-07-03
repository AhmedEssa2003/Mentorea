using Mentorea.Contracts.Common;
using Mentorea.Contracts.Posts;
using Mentorea.Extension;
using Mentorea.Services;


namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class PostController(IPostService postService) : ControllerBase
    {
        private readonly IPostService _postService = postService;
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery]RequestFilters request ,CancellationToken cancellationToken)
        {
            return Ok(await _postService.GetAllSync(request,cancellationToken));
        }
        [HttpGet("{id}")]
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        public async Task<IActionResult> Get([FromRoute]string id, CancellationToken cancellationToken)
        {
            var result = await _postService.GetSync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpGet("followed")]
        [Authorize]
        public async Task<IActionResult> GetFollowedPosts([FromQuery]RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await _postService.GetFollowedPostsAsync(User.GetUserId()!,request,cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentor}")]
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody]PostRequest request, CancellationToken cancellationToken)
        {
            var result = await _postService.CreateSync(User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {result.Value().Id},result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentor}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]string id,[FromBody] PostRequest request, CancellationToken cancellationToken)
        {
            var result = await _postService.UpdateSync(id,User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentor}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _postService.DeleteSync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
