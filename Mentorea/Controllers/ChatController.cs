using Mentorea.Contracts.Chat;
using Mentorea.Contracts.Common;


namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("default")]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        private readonly IChatService _chatService = chatService;

        [HttpGet("history/{receiverId}")]
        public async Task<IActionResult> GetMessages([FromQuery]RequestFilters requestFilters ,string receiverId)
        {
            var result = await _chatService.GetChatHistoryAsync(requestFilters ,User.GetUserId()!, receiverId);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }

        [HttpPost("")]
        public async Task<IActionResult> UploadFile([FromForm] FileRequest request)
        {
            var result = await _chatService.SaveFileAsync(request);
            return result.IsSuccess? Ok(result.Value()): result.ToProblem();
        }

        [HttpDelete("{MessageId}")]
        public async Task<IActionResult> DeleteMessage([FromRoute]string MessageId)
        {
            var result = await _chatService.DeleteMessage(MessageId,User.GetUserId()!);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("DeleteChat/{userId}")]
        public async Task<IActionResult> DeleteChat([FromRoute]string userId)
        {
            var result = await _chatService.DeleteChatAsync(User.GetUserId()!, userId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

    }
}
