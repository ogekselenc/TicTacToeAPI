using System.Collections.Generic;
using System.Linq;
using TicTacToeAPI.Data;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Repositories
{

    public class PlayerRepository : IPlayerRepository
    {
        private readonly TicTacToeDbContext _context;

        public PlayerRepository(TicTacToeDbContext context)
        {
            _context = context;
        }

        public Player GetById(int playerId) => _context.Players.Find(playerId);
        public IEnumerable<Player> GetAll() => _context.Players.ToList();
        public void Add(Player player) => _context.Players.Add(player);
    }
}