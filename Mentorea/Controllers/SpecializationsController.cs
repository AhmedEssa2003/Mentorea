using Mentorea.Contracts.Specializations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class SpecializationsController(ISpecializationService specializationService) : ControllerBase
    {
        private readonly ISpecializationService _specializationService = specializationService;
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            
            var result = await _specializationService.GetAllAsync(cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]string id,CancellationToken cancellationToken)
        {
            var result = await _specializationService.GetByIdAsync(id,cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]SpecializationRequest request,CancellationToken cancellationToken)
        {
            var result = await _specializationService.CreateAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value().Id }, result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]string id,[FromBody] SpecializationRequest request,CancellationToken cancellationToken)
        {
            var result = await _specializationService.UpdateAsync(id, request,cancellationToken );
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]string id ,CancellationToken cancellationToken)
        {
            var result = await _specializationService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
