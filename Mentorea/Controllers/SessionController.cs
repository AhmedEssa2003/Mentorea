using Mentorea.Abstractions.Enums;
using Mentorea.Contracts.Common;
using Mentorea.Contracts.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class SessionController(ISessionService sessionService) : ControllerBase
    {
        private readonly ISessionService _sessionService = sessionService;

        [HttpGet("")]
        [Authorize(Roles = DefaultRole.Admin)]
        public async Task<IActionResult> GetAll([FromQuery]RequestFilters request ,CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetAllAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles =$"{DefaultRole.Mentee},{DefaultRole.Mentor}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetAllByUserWithStatus([FromQuery]RequestFilters requestFilters, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetSessionByUserIdWithStatusAsync(User.GetUserId()!,requestFilters, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] SessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.AddAsync(request, User.GetUserId()!, cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value().Id }, result.Value()) : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpPut("{id}/respond")]
        public async Task<IActionResult> RespondToSessionBeforePayment([FromRoute] string id, [FromBody] ResponseSessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.RespondToSessionBeforePayment(id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPut("{id}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.ConfirmPaymentAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentor)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateSessionRequest request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.UpdateSessionAsync(id, User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPut("{id}/respond-to-update")]
        public async Task<IActionResult> RespondsToUpdate([FromRoute] string id, [FromBody] ResponseUpdateReques request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.RespondToUpdateAsync(id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = $"{DefaultRole.Mentor},{DefaultRole.Mentee}")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.CancelSessionAsync(id,User.GetUserId()! ,cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPut("{id}/feedback")]
        public async Task<IActionResult> GiveFeedback([FromRoute] string id, [FromBody] FeedbackRequest request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GiveFeedbackAsync(id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("{id}/attended")]
        [Authorize]
        public async Task<IActionResult> AttendByOneParty([FromRoute] string id,  CancellationToken cancellationToken)
        {
            var result = await _sessionService.AttendByOnePartyAsync(id, User.GetUserId()!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Admin)]
        [HttpPut("{id}/outcome")]
        public async Task<IActionResult> SetSessionOutcome([FromRoute] string id, [FromBody] SessionOutcomeRequest request, CancellationToken cancellationToken)
        {
            var result = await _sessionService.SetSessionOutcomeAsync(id, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [Authorize(Roles = DefaultRole.Mentee)]
        [HttpPut("{id}/report")]
        public async Task<IActionResult> SubmitSessionReport([FromRoute] string id, CancellationToken cancellationToken)
        {
            var result = await _sessionService.SubmitSessionReportAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}

