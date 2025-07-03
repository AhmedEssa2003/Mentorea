using Mentorea.Contracts.Common;
using Mentorea.Services;
using Microsoft.AspNetCore.Authorization;


namespace Mentorea.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        
        [HttpPost ("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request,CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request,cancellationToken );
            return result.IsSuccess ? Ok (result.Value()) : result.ToProblem();
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetRefreshTokenAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpPut("Revoke")]
        public async Task<IActionResult> RevokeRefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPost("Mentor-Register")]
        public async Task<IActionResult> MentorRegister([FromForm] MentorRegisterRequest request,CancellationToken cancellationToken)
        {
            var result = await _authService.MentorRegisterAsync(request,cancellationToken);
            return result.IsSuccess ? Ok("Check your Email to Confirm this Acount") : result.ToProblem();
        }
        [HttpPost("Confirm-Email")]
        public async Task<IActionResult> ConfimEmail([FromQuery] ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmEmailAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        [HttpPost("Resend-Confirm-Email")]
        public async Task<IActionResult> ConfimEmail([FromBody] ResendConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.ResendConfirmEmailAsync(request, cancellationToken);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        [HttpPost("Mentee-Register")]
        public async Task<IActionResult> MenteeRegister([FromForm] MenteeRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.MenteeRegisterAsync(request,cancellationToken);
            return result.IsSuccess ? Ok("Check your Email to Confirm this Acount") : result.ToProblem();
        }
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> SendRestCodePassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await _authService.SendResetPasswordCodeAsync(request);
            return result.IsSuccess ? Ok("Check your Email") : result.ToProblem();
        }
        [HttpPost("RestPassword")]
        public async Task<IActionResult> RestPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.RestPasswordAsync(request);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }
        
    }
}
