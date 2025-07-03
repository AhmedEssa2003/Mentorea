using Mentorea.Contracts.MentorAvailability;


namespace Mentorea.Controllers
{
    [Route("api/mentors")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class AvailabilityTimeController(IMentorAvailabilityService mentorAvailabilityService) : ControllerBase
    {
        private readonly IMentorAvailabilityService _mentorAvailabilityService = mentorAvailabilityService;
        [Authorize(Roles = $"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("{mentorId}/availability")]
        public async Task<IActionResult> GetAll([FromRoute] string mentorId, CancellationToken cancellationToken)
        {
            var result = await _mentorAvailabilityService.GetAllAsync(mentorId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpGet("{mentorId}/availability/{id}")]
        public async Task<IActionResult> Get([FromRoute] string mentorId, [FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _mentorAvailabilityService.GetAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] MentorAvailabilityRequest request, CancellationToken cancellationToken)
        {
            var result = await _mentorAvailabilityService.CreateAsync(User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { mentorId = User.GetUserId()!, id = result.Value().Id }, result.Value())
                : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] MentorAvailabilityRequest request, CancellationToken cancellationToken)
        {
            var result = await _mentorAvailabilityService.UpdateAsync(id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _mentorAvailabilityService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }

}
