using System.ComponentModel.DataAnnotations;

namespace TicTacToeAPI.DTOs;

// DTO for creating a new player
public class CreatePlayerDTO
{
    [Required(ErrorMessage = "Player name is required")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
    public string Name { get; set; }
}

// DTO for returning player information
public class PlayerDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}