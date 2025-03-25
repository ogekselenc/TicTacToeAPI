namespace TicTacToeAPI.Interfaces;
using TicTacToeAPI.Models;

public interface IUnitOfWork : IDisposable
{
    IRepository<Game> Games { get; }
    IRepository<Move> Moves { get; }
    IRepository<Player> Players { get; }
    Task<int> CompleteAsync();
}