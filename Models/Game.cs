using System;
using System.Collections.Generic;

namespace TicTacToeAPI.Models;

public partial class Game
{
    public int Id { get; set; }

    public int PlayerXid { get; set; }

    public int? PlayerOid { get; set; }

    public int BoardSize { get; set; }

    public int WinningLine { get; set; }

    public string OutcomeStatus { get; set; } = null!;

    public string OutcomeReason { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? PlayerId { get; set; }

    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();

    public virtual Player? Player { get; set; }

    public virtual Player? PlayerO { get; set; }

    public virtual Player PlayerX { get; set; } = null!;
}
