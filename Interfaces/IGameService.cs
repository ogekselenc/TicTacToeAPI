using TicTacToeAPI.DTOs;

namespace TicTacToeAPI.Interfaces;

public interface IGameService
{
    Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto);
    Task<GameDTO> JoinGameAsync(int gameId, JoinGameDTO joinGameDto);
    Task<GameDTO> MakeMoveAsync(int gameId, MakeMoveDTO makeMoveDto);
    Task<GameDTO> GetGameAsync(int gameId);
    Task<MoveDTO> GetMoveAsync(int moveId);
    Task<GameDTO> ResignGameAsync(int gameId, int playerId);
    Task<IEnumerable<GameDTO>> GetActiveGamesAsync();
    Task<IEnumerable<MoveDTO>> GetGameMovesAsync(int gameId);
}