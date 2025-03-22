using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.Models;
using TicTacToeAPI.Services;

[Route("api/game")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("create")]
    public IActionResult CreateGame([FromBody] Player player, int boardSize, int winningLine)
    {
        var game = _gameService.CreateGame(player, boardSize, winningLine);
        return Ok(game);
    }

    [HttpPost("join/{gameId}")]
    public IActionResult JoinGame(int gameId, [FromBody] Player player)
    {
        _gameService.JoinGame(gameId, player);
        return Ok();
    }
}
