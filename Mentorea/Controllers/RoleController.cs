using Mentorea.Contracts.Roles;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRole.Admin)]
    [EnableRateLimiting("default")]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;
        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var Roles = await _roleService.GetAllAsync(cancellationToken);
            return Ok(Roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _roleService.GetAsync(id);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] RoleRequest request)
        {
            var result = await _roleService.AddAsync(request);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {id = result.Value().Id }, result.Value()) : result.ToProblem();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request)
        {
            var result = await _roleService.UpdateAsync(id, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _roleService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
