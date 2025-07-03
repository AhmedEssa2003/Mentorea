using Mentorea.Contracts.Comments;
using Mentorea.Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mentorea.Controllers
{
    [Route("api/Posts/{postId}/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
    public class CommentController(ICommentService commentService) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery]RequestFilters requestFilters,[FromRoute]string postId, CancellationToken cancellationToken)
        {
            var result = await _commentService.GetAllAsync(requestFilters, postId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();

        }
        [HttpGet("{CommentId}")]
        public async Task<IActionResult> Get([FromRoute] string postId, [FromRoute] string CommentId, CancellationToken cancellationToken)
        {
            var result = await _commentService.GetAsync(postId, CommentId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpPost("")]
        public async Task<IActionResult> Create([FromRoute] string postId, [FromBody] CommentRequest request, CancellationToken cancellationToken)
        {
            var result = await _commentService.CreateAsync(postId, User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? 
                CreatedAtAction(nameof(Get), new { postId, CommentId = result.Value().Id }, result.Value())
                : result.ToProblem();
        }
        [HttpPut("{CommentId}")]
        public async Task<IActionResult> Update([FromRoute] string postId, [FromRoute] string CommentId, [FromBody] CommentRequest request, CancellationToken cancellationToken)
        {
            var result = await _commentService.UpdateAsync(postId, CommentId, User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpDelete("{CommentId}")]
        public async Task<IActionResult> Delete([FromRoute] string postId, [FromRoute] string CommentId, CancellationToken cancellationToken)
        {
            var result = await _commentService.DeleteAsync(postId, CommentId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
