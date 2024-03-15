using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Song
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Artist { get; set; } = null!;

    public string? Modifiers { get; set; }

    public string Charter { get; set; } = null!;

    public string Source { get; set; } = null!;

    public int FocusId { get; set; }

    public virtual ICollection<Focus> Foci { get; set; } = new List<Focus>();

    public virtual Focus Focus { get; set; } = null!;

    public virtual ICollection<Match> MatchPlayer1BanSongs { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchPlayer2BanSongs { get; set; } = new List<Match>();

    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
