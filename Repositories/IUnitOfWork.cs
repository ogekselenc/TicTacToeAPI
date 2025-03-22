public interface IUnitOfWork
{
    IGameRepository GameRepository { get; }
    IPlayerRepository PlayerRepository { get; }
    IMoveRepository MoveRepository { get; }
    void Save();
}
