using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

namespace TicTacToeAPI.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly PlayerRepository _playerRepository;

        public PlayerController(PlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] Player player)
        {
            var newPlayer = await _playerRepository.CreatePlayerAsync(player);
            return CreatedAtAction(nameof(GetPlayerByUsername), new { username = newPlayer.Username }, newPlayer);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetPlayerByUsername(string username)
        {
            var player = await _playerRepository.GetPlayerByUsernameAsync(username);
            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playerRepository.GetAllPlayersAsync();
            return Ok(players);
        }
    }
}
