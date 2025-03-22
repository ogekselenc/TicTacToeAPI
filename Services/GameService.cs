using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

public class GameService
{
    private readonly IUnitOfWork _unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Game CreateGame(Player player, int boardSize, int winningLine)
    {
        var game = new Game
        {
            BoardSize = boardSize,
            WinningLineLength = winningLine,
            XPlayerId = player.Id,
            Status = "WaitingForOpponent"
        };

        _unitOfWork.GameRepository.Add(game);
        _unitOfWork.Save();
        return game;
    }

    public void JoinGame(int gameId, Player player)
    {
        var game = _unitOfWork.GameRepository.GetById(gameId);
        if (game != null && game.OPlayerId == null)
        {
            game.OPlayerId = player.Id;
            game.Status = "InProgress";
            _unitOfWork.GameRepository.Update(game);
            _unitOfWork.Save();
        }
    }
}
