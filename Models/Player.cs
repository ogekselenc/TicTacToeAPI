using System;
using System.Collections.Generic;

namespace TicTacToeAPI.Models;

public partial class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Game> GamePlayerOs { get; set; } = new List<Game>();

    public virtual ICollection<Game> GamePlayerXes { get; set; } = new List<Game>();

    public virtual ICollection<Game> GamePlayers { get; set; } = new List<Game>();

    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();
}
