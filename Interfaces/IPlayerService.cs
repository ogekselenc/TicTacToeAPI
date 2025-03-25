using TicTacToeAPI.DTOs;

namespace TicTacToeAPI.Interfaces;

public interface IPlayerService
{
    Task<PlayerDTO> CreatePlayerAsync(CreatePlayerDTO createPlayerDto);
    Task<PlayerDTO> GetPlayerAsync(int id);
    Task<IEnumerable<PlayerDTO>> GetPlayersAsync();
}