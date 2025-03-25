using TicTacToeAPI.Interfaces;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<Game> _games;
    private IRepository<Move> _moves;
    private IRepository<Player> _players;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IRepository<Game> Games => _games ??= new Repository<Game>(_context);
    public IRepository<Move> Moves => _moves ??= new Repository<Move>(_context);
    public IRepository<Player> Players => _players ??= new Repository<Player>(_context);

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}