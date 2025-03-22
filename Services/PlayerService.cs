using TicTacToeAPI.Models;
using TicTacToeAPI.Repositories;

namespace TicTacToeAPI.Services
{
    public class PlayerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Player RegisterPlayer(string name)
        {
            var player = new Player { Name = name };
            _unitOfWork.PlayerRepository.Add(player);
            _unitOfWork.Save();
            return player;
        }
    }

}