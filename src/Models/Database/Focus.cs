using System;
using System.Collections.Generic;

namespace CSCCSTRDB.Models.Database;

public partial class Focus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? TiebreakSongId { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();

    public virtual Song? TiebreakSong { get; set; }
}
