using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Interfaces;

namespace TicTacToeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Creates a new game
    /// </summary>
    /// <param name="createGameDto">Game creation data</param>
    /// <returns>The newly created game</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDTO>> CreateGame([FromBody] CreateGameDTO createGameDto)
    {
        var game = await _gameService.CreateGameAsync(createGameDto);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    /// <summary>
    /// Gets a game by ID
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>The requested game</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDTO>> GetGame(int id)
    {
        var game = await _gameService.GetGameAsync(id);
        return Ok(game);
    }

    /// <summary>
    /// Gets all active games
    /// </summary>
    /// <returns>List of active games</returns>
    [HttpGet("active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GameDTO>>> GetActiveGames()
    {
        var games = await _gameService.GetActiveGamesAsync();
        return Ok(games);
    }

    /// <summary>
    /// Joins an existing game as Player O
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <param name="joinGameDto">Player O information</param>
    /// <returns>The updated game</returns>
    [HttpPost("{id}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDTO>> JoinGame(int id, [FromBody] JoinGameDTO joinGameDto)
    {
        var game = await _gameService.JoinGameAsync(id, joinGameDto);
        return Ok(game);
    }

    /// <summary>
    /// Makes a move in a game
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <param name="makeMoveDto">Move details</param>
    /// <returns>The updated game state</returns>
    [HttpPost("{id}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GameDTO>> MakeMove(int id, [FromBody] MakeMoveDTO makeMoveDto)
    {
        var game = await _gameService.MakeMoveAsync(id, makeMoveDto);
        return Ok(game);
    }

    /// <summary>
    /// Gets all moves for a game
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>List of moves</returns>
    [HttpGet("{id}/moves")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MoveDTO>>> GetGameMoves(int id)
    {
        var moves = await _gameService.GetGameMovesAsync(id);
        return Ok(moves);
    }
}