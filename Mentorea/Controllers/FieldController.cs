using Mentorea.Abstractions;
using Mentorea.Contracts.Fields;
using Mentorea.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class FieldController (IFieldService fieldService): ControllerBase
    {
        private readonly IFieldService _fieldService = fieldService;
        [HttpGet("")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _fieldService.GetAll(cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get([FromRoute]string Id,CancellationToken cancellationToken)
        {
            var result = await _fieldService.GetAsync(Id,cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody]FieldRequest request, CancellationToken cancellationToken)
        {
            var result = await _fieldService.CreateAsync(request, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(Get),new { result.Value().Id }, result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]string id,[FromBody] FieldRequest request, CancellationToken cancellationToken)
        {
            var result = await _fieldService.UpdateAsync(id,request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] string Id, CancellationToken cancellationToken)
        {
            var result = await _fieldService.DeleteAsync(Id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}