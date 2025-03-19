using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

namespace TicTacToeAPI.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly GameRepository _gameRepository;

        public GameController(GameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            var newGame = await _gameRepository.CreateGameAsync(game);
            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _gameRepository.GetGameByIdAsync(id);
            if (game == null)
                return NotFound();

            return Ok(game);
        }

        [HttpPost("{id}/move")]
        public async Task<IActionResult> MakeMove(int id, [FromBody] MoveRequest request)
        {
            var game = await _gameRepository.GetGameByIdAsync(id);
            if (game == null)
                return NotFound("Game not found.");

            if (!game.MakeMove(request.Row, request.Col, request.Player))
                return BadRequest("Invalid move.");

            await _gameRepository.UpdateGameAsync(game);
            return Ok(game);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _gameRepository.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            await _gameRepository.DeleteGameAsync(id);
            return NoContent();
        }
    }
}
