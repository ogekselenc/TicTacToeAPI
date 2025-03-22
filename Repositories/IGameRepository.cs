using System.Collections.Generic;
using TicTacToeAPI.Models;

public interface IGameRepository
{
    Game GetById(int gameId);
    IEnumerable<Game> GetAll();
    void Add(Game game);
    void Update(Game game);
}
