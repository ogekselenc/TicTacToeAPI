using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Interfaces;

namespace TicTacToeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    /// <summary>
    /// Creates a new player
    /// </summary>
    /// <param name="createPlayerDto">Player creation data</param>
    /// <returns>The newly created player</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlayerDTO>> CreatePlayer([FromBody] CreatePlayerDTO createPlayerDto)
    {
        var player = await _playerService.CreatePlayerAsync(createPlayerDto);
        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }

    /// <summary>
    /// Gets a player by ID
    /// </summary>
    /// <param name="id">Player ID</param>
    /// <returns>The requested player</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlayerDTO>> GetPlayer(int id)
    {
        var player = await _playerService.GetPlayerAsync(id);
        return Ok(player);
    }

    /// <summary>
    /// Gets all players
    /// </summary>
    /// <returns>List of all players</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetPlayers()
    {
        var players = await _playerService.GetPlayersAsync();
        return Ok(players);
    }
}