using Mentorea.Contracts.Card;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mentorea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController(ICardService cardService) : ControllerBase
    {
        private readonly ICardService _cardService = cardService;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id,CancellationToken cancellationToken)
        {
            var result = await _cardService.GetAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem(); 
        }
        [HttpPost("")]
        public async Task<IActionResult> Creat(CardRequest request, CancellationToken cancellationToken)
        {
            var result = await _cardService.CreateAsync(User.GetUserId()!,request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value()) : result.ToProblem();
        }
        [HttpPut("")]
        public async Task<IActionResult> Update (CardRequest request, CancellationToken cancellationToken)
        {
            var result = await _cardService.UpdateAsync(User.GetUserId()!, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpDelete("")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var result = await _cardService.DeleteAsync(User.GetUserId()!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

    }
}
