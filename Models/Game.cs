using System;
using System.Collections.Generic;
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
        public string Player1 { get; set; } = string.Empty; // ✅ Default empty string

        [Required]
        public string Player2 { get; set; } = string.Empty; // ✅ Default empty string

        public int Size { get; set; } = 3;
        public int WinLength { get; set; } = 3;

        [Required]
        public string BoardState { get; set; } = "[]"; // ✅ Default empty board

        public string CurrentPlayer { get; set; } = "X";
        public bool IsGameOver { get; set; } = false;

        [Required]
        public string Winner { get; set; } = "A"; // ✅ Default empty winner

        // ✅ Initialize Board
        public void InitializeBoard()
        {
            List<List<char>> board = new List<List<char>>();
            for (int i = 0; i < Size; i++)
            {
                List<char> row = new List<char>();
                for (int j = 0; j < Size; j++)
                {
                    row.Add('A');
                }
                board.Add(row);
            }
            BoardState = JsonSerializer.Serialize(board);
        }

        // ✅ Convert Board JSON String to List<List<char>>
        public List<List<char>> GetBoard()
        {
            try
            {
                return JsonSerializer.Deserialize<List<List<char>>>(BoardState) ?? new List<List<char>>();
            }
            catch (Exception)
            {
                return new List<List<char>>();
            }
        } 

        // ✅ Convert List<List<char>> Back to JSON String
        public void SetBoard(List<List<char>> board)
        {
            BoardState = JsonSerializer.Serialize(board);
        }
    }
}
