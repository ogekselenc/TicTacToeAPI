using System;
using System.Collections.Generic;

namespace TicTacToeAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerXId { get; set; }
        public int? PlayerOId { get; set; } // Nullable until opponent joins
        public int BoardSize { get; set; }
        public int WinningLine { get; set; }
        public string OutcomeStatus { get; set; }
        public string OutcomeReason { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Player PlayerX { get; set; }
        public Player PlayerO { get; set; }
        public List<Move> Moves { get; set; }
    }
}
