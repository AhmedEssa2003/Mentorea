using Mentorea.Contracts.TokenDevice;


namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeviceTokenController(IFcmService fcmService) : ControllerBase
    {
        private readonly IFcmService _fcmService = fcmService;

        [HttpPost("add-token")]
        public async Task<IActionResult> AddTokenDevice([FromBody] TokenDeviceRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _fcmService.AddTokenDevice(request, cancellationToken);
            return result.IsSuccess? Ok() : result.ToProblem();
        }
        
    }
}
