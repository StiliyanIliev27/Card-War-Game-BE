using Microsoft.AspNetCore.Mvc;

namespace WarGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly IDeckService deckService;

        public DeckController(IDeckService deckService)
        {
            this.deckService = deckService;
        }

        [HttpGet("deal")]
        public IActionResult DealCards()
        {
            var players = deckService.DealCards();
            return Ok(players);
        }

        [HttpPost("play")]
        public IActionResult PlayRound()
        {
            try
            {
                var response = deckService.Play();
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
