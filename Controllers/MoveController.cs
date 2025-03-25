using Microsoft.AspNetCore.Mvc;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Interfaces;

namespace TicTacToeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovesController : ControllerBase
{
    private readonly IGameService _gameService;

    public MovesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Gets a specific move
    /// </summary>
    /// <param name="id">Move ID</param>
    /// <returns>The requested move</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MoveDTO>> GetMove(int id)
    {
        var move = await _gameService.GetMoveAsync(id);
        return Ok(move);
    }
}