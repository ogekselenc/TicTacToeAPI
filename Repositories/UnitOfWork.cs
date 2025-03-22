using TicTacToeAPI.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicTacToeDbContext _context;
    public IGameRepository GameRepository { get; }
    public IPlayerRepository PlayerRepository { get; }
    public IMoveRepository MoveRepository { get; }

    public UnitOfWork(TicTacToeDbContext context, IGameRepository gameRepo, IPlayerRepository playerRepo, IMoveRepository moveRepo)
    {
        _context = context;
        GameRepository = gameRepo;
        PlayerRepository = playerRepo;
        MoveRepository = moveRepo;
    }

    public void Save() => _context.SaveChanges();
}
