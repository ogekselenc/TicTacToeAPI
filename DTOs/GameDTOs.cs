using System.ComponentModel.DataAnnotations;

namespace TicTacToeAPI.DTOs;

// DTO for creating a new game
public class CreateGameDTO
{
    [Required(ErrorMessage = "Player X ID is required")]
    public int PlayerXId { get; set; }

    public int? PlayerOId { get; set; } // Optional for joining later

    [Range(3, 10, ErrorMessage = "Board size must be between 3 and 10")]
    public int BoardSize { get; set; } = 3;

    [Range(3, 10, ErrorMessage = "Winning line must be between 3 and 10")]
    public int WinningLine { get; set; } = 3;
}

// DTO for returning game information
public class GameDTO
{
    public int Id { get; set; }
    public PlayerDTO PlayerX { get; set; }
    public PlayerDTO PlayerO { get; set; }
    public int BoardSize { get; set; }
    public int WinningLine { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MoveDTO> Moves { get; set; } = new();
}

// DTO for joining a game
public class JoinGameDTO
{
    [Required(ErrorMessage = "Player O ID is required")]
    public int PlayerOId { get; set; }
}