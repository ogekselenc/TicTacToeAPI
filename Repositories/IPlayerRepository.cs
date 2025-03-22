using System.Collections.Generic;
using TicTacToeAPI.Models;

public interface IPlayerRepository
{
    Player GetById(int playerId);
    IEnumerable<Player> GetAll();
    void Add(Player player);
}
