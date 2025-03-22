using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

public class MoveService
{
    private readonly IUnitOfWork _unitOfWork;

    public MoveService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void MakeMove(int gameId, int playerId, int x, int y)
    {
        var move = new Move
        {
            GameId = gameId,
            PlayerId = playerId,
            X = x,
            Y = y
        };

        _unitOfWork.MoveRepository.Add(move);
        _unitOfWork.Save();
    }
}
