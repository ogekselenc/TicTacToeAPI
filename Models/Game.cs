using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToeAPI.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Player1 { get; set; }

        [Required]
        public string? Player2 { get; set; }

        public string BoardState { get; set; } = "---------"; // Ensures default board is always a string

        public string CurrentPlayer { get; set; } = "X"; // Default first player

        public bool IsGameOver { get; set; }
        public string? Winner { get; set; }
    }

}
