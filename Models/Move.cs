using System;
using System.Collections.Generic;

namespace TicTacToeAPI.Models;

public partial class Move
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int PlayerId { get; set; }

    public int PositionX { get; set; }

    public int PositionY { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
