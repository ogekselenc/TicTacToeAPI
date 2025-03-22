using System.Collections.Generic;
using TicTacToeAPI.Models;

public interface IMoveRepository
{
    Move GetById(int moveId);
    IEnumerable<Move> GetByGameId(int gameId);
    void Add(Move move);
}
