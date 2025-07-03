using Mentorea.Contracts.Users;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRole.Admin)]
    [EnableRateLimiting("default")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _userService.GetAllAsync(cancellationToken));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _userService.GetAsync(id);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.AddAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value().Id }, result.Value()) : result.ToProblem();
        }
        
        [HttpPut("{Id}/UnLock")]
        public async Task<IActionResult> UnLock([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var result = await _userService.UnLockAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{Id}/ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var result = await _userService.ToggleStatusAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
