using System.ComponentModel.DataAnnotations;

namespace TicTacToeAPI.DTOs;

// DTO for making a move
public class MakeMoveDTO
{
    [Required(ErrorMessage = "Player ID is required")]
    public int PlayerId { get; set; }

    [Required(ErrorMessage = "X position is required")]
    [Range(0, 10, ErrorMessage = "X position must be between 0 and 10")]
    public int PositionX { get; set; }

    [Required(ErrorMessage = "Y position is required")]
    [Range(0, 10, ErrorMessage = "Y position must be between 0 and 10")]
    public int PositionY { get; set; }
}

// DTO for returning move information
public class MoveDTO
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public DateTime CreatedAt { get; set; }
}