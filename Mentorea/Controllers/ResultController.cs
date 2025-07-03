using Mentorea.Contracts.Common;
using Microsoft.AspNetCore.RateLimiting;


namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]

    public class ResultController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery] RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetAllMentorsAsync(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("Specialization")]
        public async Task<IActionResult> GetBySpecialization([FromQuery] RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await _resultService.MentorsBySpecializationAsync(request, cancellationToken);
            return Ok(result);
        }
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpGet("Recommended")]
        public async Task<IActionResult> GetRecommendedMentors([FromQuery] RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetRecomendedMentors(User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
    }
}
