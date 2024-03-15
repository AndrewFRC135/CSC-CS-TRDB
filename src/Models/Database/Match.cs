using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Match
{
    public int Id { get; set; }

    public DateTime StartTimestamp { get; set; }

    public int GroupId { get; set; }

    public int Player1Id { get; set; }

    public int Player2Id { get; set; }

    public int RefereeId { get; set; }

    public int? Player1BanSongId { get; set; }

    public int? Player2BanSongId { get; set; }

    public int Player1WinCount { get; set; }

    public int Player2WinCount { get; set; }

    public int? WinningPlayerNum { get; set; }

    public DateTime? EndTimestamp { get; set; }

    public string? RefereeNotes { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Participant Player1 { get; set; } = null!;

    public virtual Song? Player1BanSong { get; set; }

    public virtual Participant Player2 { get; set; } = null!;

    public virtual Song? Player2BanSong { get; set; }

    public virtual Participant Referee { get; set; } = null!;

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
