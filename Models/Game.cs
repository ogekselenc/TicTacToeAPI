using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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

        public int Size { get; set; }  // Dynamic board size
        public int WinLength { get; set; } // Win condition (e.g., 3 in a 3x3 game)

        public string? BoardState { get; set; } // JSON-encoded board
        public string CurrentPlayer { get; set; } = "X";

        public bool IsGameOver { get; set; }
        public string? Winner { get; set; }

        public void InitializeBoard()
        {
            char[,] board = new char[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    board[i, j] = '-';
                }
            }
            BoardState = JsonSerializer.Serialize(board); // Convert board to JSON string
        }

        public char[,] GetBoard()
        {
            if (BoardState == null)
            {
                throw new InvalidOperationException("BoardState is null.");
            }
            return JsonSerializer.Deserialize<char[,]>(BoardState) ?? throw new InvalidOperationException("Deserialization returned null.");
        }

        public void SetBoard(char[,] board)
        {
            BoardState = JsonSerializer.Serialize(board);
        }
    }
}
