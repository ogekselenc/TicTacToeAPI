using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.Services;

namespace TicTacToeAPI.Controllers
{

    [Route("api/move")]
    [ApiController]
    public class MoveController : ControllerBase
    {
        private readonly MoveService _moveService;

        public MoveController(MoveService moveService)
        {
            _moveService = moveService;
        }

        [HttpPost("play")]
        public IActionResult MakeMove(int gameId, int playerId, int x, int y)
        {
            _moveService.MakeMove(gameId, playerId, x, y);
            return Ok();
        }
    }
}