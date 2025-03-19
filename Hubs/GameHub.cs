using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

namespace TicTacToeAPI.Hubs
{
    public class GameHub : Hub
    {
        private readonly GameRepository _gameRepository;
        private readonly ILogger<GameHub> _logger;

        public GameHub(GameRepository gameRepository, ILogger<GameHub> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task MakeMove(int gameId, int row, int col, string player)
        {
            _logger.LogInformation($"Move attempt: Game {gameId}, Player {player}, Position [{row},{col}]");

            var game = await _gameRepository.GetGameByIdAsync(gameId);
            if (game == null)
            {
                _logger.LogWarning($"Game {gameId} not found.");
                await Clients.Caller.SendAsync("Error", "Game not found.");
                return;
            }

            if (!game.MakeMove(row, col, player))
            {
                _logger.LogWarning($"Invalid move in Game {gameId} by Player {player} at [{row},{col}].");
                await Clients.Caller.SendAsync("Error", "Invalid move.");
                return;
            }

            await _gameRepository.UpdateGameAsync(game);

            _logger.LogInformation($"Move successful: Game {gameId}, Player {player}, Position [{row},{col}]");
            await Clients.Group($"game-{gameId}").SendAsync("ReceiveMove", game);
        }

        public async Task JoinGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"game-{gameId}");
            var game = await _gameRepository.GetGameByIdAsync(gameId);

            _logger.LogInformation($"Player joined Game {gameId}.");
            await Clients.Caller.SendAsync("GameState", game);
        }
    }
}
