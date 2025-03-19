using System.Collections.Generic;

namespace TicTacToeAPI.Models
{
    public class Player
    {
        public int Id { get; set; }  // Primary Key
        public string Username { get; set; }  // Unique Player username
        public int GamesPlayed { get; set; }  // Total games played
        public int GamesWon { get; set; }  // Total games won
    }
}
