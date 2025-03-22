using System;

namespace TicTacToeAPI.Models
{
    public class Move
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Game Game { get; set; }
        public Player Player { get; set; }
    }
}
