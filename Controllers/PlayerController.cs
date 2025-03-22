using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.Models;
using TicTacToeAPI.Services;

namespace TicTacToeAPI.Controllers
{

    [Route("api/player")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerService _playerService;

        public PlayerController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("register")]
        public IActionResult RegisterPlayer([FromBody] string name)
        {
            var player = _playerService.RegisterPlayer(name);
            return Ok(player);
        }
    }
}