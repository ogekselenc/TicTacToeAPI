using System.Collections.Generic;
using System.Linq;
using TicTacToeAPI.Data;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Repositories
{
    public class MoveRepository : IMoveRepository
    {
        private readonly TicTacToeDbContext _context;

        public MoveRepository(TicTacToeDbContext context)
        {
            _context = context;
        }

        public Move GetById(int moveId) => _context.Moves.Find(moveId);
        public IEnumerable<Move> GetByGameId(int gameId) => _context.Moves.Where(m => m.GameId == gameId).ToList();
        public void Add(Move move) => _context.Moves.Add(move);
    }
}