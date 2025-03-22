using System.Collections.Generic;
using System.Linq;
using TicTacToeAPI.Data;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Repositories
{

public class GameRepository : IGameRepository
{
    private readonly TicTacToeDbContext _context;

    public GameRepository(TicTacToeDbContext context)
    {
        _context = context;
    }

    public Game GetById(int gameId) => _context.Games.Find(gameId);
    public IEnumerable<Game> GetAll() => _context.Games.ToList();
    public void Add(Game game) => _context.Games.Add(game);
    public void Update(Game game) => _context.Games.Update(game);
}
}