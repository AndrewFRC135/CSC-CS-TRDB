using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Round
{
    public int MatchId { get; set; }

    public int RoundNum { get; set; }

    public int SeqNum { get; set; }

    public DateTime Timestamp { get; set; }

    public int? PickingPlayerNum { get; set; }

    public int? PickedSongId { get; set; }

    public byte[]? ResultsScreenshot { get; set; }

    public int? Player1Score { get; set; }

    public int? Player2Score { get; set; }

    public int? WinningPlayerNum { get; set; }

    public int? ForefitPlayerNum { get; set; }

    public string? RefereeNotes { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Song? PickedSong { get; set; }
}
