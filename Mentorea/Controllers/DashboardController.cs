using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRole.Admin)]
    public class DashboardController(IDashboardService dashboardService) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;

        [HttpGet("mentor")]
        public async Task<IActionResult> GetNumberOfMentor(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.CountOfMentorAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("mentee")]
        public async Task<IActionResult> GetNumberOfMentee(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.CountOfMenteeAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("post")]
        public async Task<IActionResult> GetNumberOfPost(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.CountOfPostAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("session")]
        public async Task<IActionResult> GetNumberOfSession(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.CountOfSessionAsync(cancellationToken);
            return Ok(result);
        }
        [HttpGet("payment-recipient")]
        public async Task<IActionResult> GetPaymentRecipient(CancellationToken cancellationToken)
        {
            var result = await _dashboardService.GetPaymentRecipientAsync(cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpDelete("payment-recipient/{sessionId}")]
        public async Task<IActionResult> DeleteFromList(string sessionId, CancellationToken cancellationToken)
        {
            var result = await _dashboardService.DeleteFromListAsync(sessionId, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
