using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Participant
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Pronouns { get; set; }

    public string? DiscordId { get; set; }

    public bool? IsCompetitor { get; set; }

    public int? QualifierRank { get; set; }

    public int? QualifierScore { get; set; }

    public bool? AdvancedToPlayoff { get; set; }

    public bool? ReceivedGroupBye { get; set; }

    public bool? IsReferee { get; set; }

    public int? GroupId { get; set; }

    public int? GroupRank { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Match> MatchPlayer1s { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchPlayer2s { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchReferees { get; set; } = new List<Match>();
}
