using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? DiscordChannelId { get; set; }

    public int? ChServerPort { get; set; }

    public string? ChServerPassword { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
}
