using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;
using TicTacToeAPI.Services;
using TicTacToeAPI.Hubs;
using TicTacToeAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace TicTacToeAPI.Services
{
    public class GameService
    {
        private readonly TicTacToeDbContext _context;

        public GameService(TicTacToeDbContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(Player player, int boardSize, int winningLine)
        {
            var game = new Game
            {
                PlayerXId = player.Id,
                BoardSize = boardSize,
                WinningLine = winningLine,
                OutcomeStatus = "InProgress"
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task<Game> JoinGameAsync(int gameId, Player player)
        {
            var game = await _context.Games
                .Include(g => g.PlayerX)
                .Include(g => g.PlayerO)
                .FirstOrDefaultAsync(g => g.Id == gameId && g.OutcomeStatus == "InProgress");

            if (game == null)
                throw new Exception("Game not found or already completed.");

            if (game.PlayerX == null)
            {
                game.PlayerXId = player.Id;
            }
            else if (game.PlayerO == null)
            {
                game.PlayerOId = player.Id;
            }
            else
            {
                throw new Exception("Game is full.");
            }

            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Move> MakeMoveAsync(int gameId, Player player, int x, int y)
        {
            var game = await _context.Games
                .Include(g => g.Moves)
                .FirstOrDefaultAsync(g => g.Id == gameId && g.OutcomeStatus == "InProgress");

            if (game == null)
                throw new Exception("Game not found or already completed.");

            var move = new Move
            {
                GameId = gameId,
                PlayerId = player.Id,
                PositionX = x,
                PositionY = y
            };

            game.Moves.Add(move);
            await _context.SaveChangesAsync();

            return move;
        }

        public async Task<Game> EndGameAsync(int gameId, string outcomeStatus, string outcomeReason)
        {
            var game = await _context.Games
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
                throw new Exception("Game not found.");

            game.OutcomeStatus = outcomeStatus;
            game.OutcomeReason = outcomeReason;
            await _context.SaveChangesAsync();

            return game;
        }
    }

}