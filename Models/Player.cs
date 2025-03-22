using System;
using System.Collections.Generic;

namespace TicTacToeAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Game> GamesPlayed { get; set; }
    }
}
