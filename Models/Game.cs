using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;



namespace TicTacToeAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string PlayerX { get; set; } = string.Empty;
        public string PlayerO { get; set; } = string.Empty;
        public int BoardSize { get; set; }
        public int WinningLine { get; set; }
        public string CurrentTurn { get; set; } = "X";  // Start with X

        [NotMapped] // Tells EF Core to ignore this property
        public string[,] Board { get; set; }

        public string BoardJson
        {
            get => JsonSerializer.Serialize(Board);
            set => Board = JsonSerializer.Deserialize<string[,]>(value) ?? new string[0, 0];
        }
        public string Winner { get; set; } = string.Empty;
        public bool IsGameOver { get; set; } = false;

        public Game()
        {
            InitializeBoard();
        }

        public void InitializeBoard()
        {
            Board = new string[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    Board[i, j] = "";
        }

        public bool MakeMove(int row, int col, string player)
        {
            if (IsGameOver || Board[row, col] != "" || player != CurrentTurn)
                return false;

            Board[row, col] = player;
            if (CheckWinner(row, col, player))
            {
                Winner = player;
                IsGameOver = true;
            }
            else if (IsBoardFull())
            {
                IsGameOver = true;
                Winner = "Draw";
            }
            else
            {
                CurrentTurn = (CurrentTurn == "X") ? "O" : "X";
            }
            return true;
        }

        private bool IsBoardFull()
        {
            return Board.Cast<string>().All(cell => cell != "");
        }

        private bool CheckWinner(int row, int col, string player)
        {
            return CheckDirection(row, col, player, 1, 0) || // Horizontal
                   CheckDirection(row, col, player, 0, 1) || // Vertical
                   CheckDirection(row, col, player, 1, 1) || // Diagonal \
                   CheckDirection(row, col, player, 1, -1);  // Diagonal /
        }

        private bool CheckDirection(int row, int col, string player, int dRow, int dCol)
        {
            int count = 1;
            count += CountInDirection(row, col, player, dRow, dCol);
            count += CountInDirection(row, col, player, -dRow, -dCol);
            return count >= WinningLine;
        }

        private int CountInDirection(int row, int col, string player, int dRow, int dCol)
        {
            int count = 0;
            int i = row + dRow, j = col + dCol;
            while (i >= 0 && i < BoardSize && j >= 0 && j < BoardSize && Board[i, j] == player)
            {
                count++;
                i += dRow;
                j += dCol;
            }
            return count;
        }
    }
}
