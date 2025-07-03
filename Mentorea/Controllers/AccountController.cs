using Mentorea.Contracts.Users;
using Mentorea.Extension;
using Mentorea.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Mentorea.Controllers
{
    [Route("me")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class AccountController (IUserService userService): ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("Mentor")]
        [Authorize(Roles = DefaultRole.Mentor)]
        public async Task<IActionResult>MentorInfo()
        {
            var result = await _userService.GetMentorProfileAsync(User.GetUserId()!);
            return Ok(result.Value());
        }
        [HttpGet("Mentee")]
        [Authorize(Roles = DefaultRole.Mentee)]
        public async Task<IActionResult> MenteeInfo()
        {
            var result = await _userService.GetMenteeProfileAsync(User.GetUserId()!);
            return Ok(result.Value());
        }
        [HttpGet("Admin")]
        [Authorize(Roles = DefaultRole.Admin)]
        public async Task<IActionResult> AdminInfo()
        {
            var result = await _userService.GetAdminProfileAsync(User.GetUserId()!);
            return Ok(result.Value());
        }
        [HttpPut("Mentee")]
        [Authorize(Roles = DefaultRole.Mentee)]
        public async Task<IActionResult> UpdateMentee([FromBody]UpdateMenteeProfileRequest request)
        {
            var result = await _userService.UpdateMenteeProfileAsync(User.GetUserId()!,request);
            return result.IsSuccess ? NoContent() :result.ToProblem();
        }
        [HttpPut("Mentor")]
        [Authorize(Roles = DefaultRole.Mentor)]
        public async Task<IActionResult> UpdateMentor([FromBody] UpdateMentorProfileRequest request)
        {
            var result = await _userService.UpdateMentorProfileAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("Admin")]
        [Authorize(Roles = DefaultRole.Admin)]
        public async Task<IActionResult> UpdateAdmin([FromBody] UpdateAdminProfileRequest request)
        {
            var result = await _userService.UpdateAdminProfileAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("Image")]
        [Authorize]
        public async Task<IActionResult> UpdateImage([FromForm] ImageRequest request)
        {
            var result = await _userService.UpdateImageAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        
    }
}
