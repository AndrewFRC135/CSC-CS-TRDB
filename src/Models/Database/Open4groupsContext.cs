using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CSCCSTRDB.Models.Database;

public partial class Open4groupsContext : DbContext
{
    public Open4groupsContext()
    {
    }

    public Open4groupsContext(DbContextOptions<Open4groupsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Focus> Foci { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=10.16.239.10;port=36578;user=awhiteman;password=aww82104;database=open4groups", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.6-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Focus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("focus")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.Name, "name");

            entity.HasIndex(e => e.TiebreakSongId, "tiebreak_song_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.TiebreakSongId)
                .HasColumnType("int(11)")
                .HasColumnName("tiebreak_song_id");

            entity.HasOne(d => d.TiebreakSong).WithMany(p => p.Foci)
                .HasForeignKey(d => d.TiebreakSongId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("focus_ibfk_1");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("group")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DiscordChannelId, "discord_channel_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.ChServerPassword)
                .HasMaxLength(255)
                .HasColumnName("ch_server_password");
            entity.Property(e => e.ChServerPort)
                .HasColumnType("int(11)")
                .HasColumnName("ch_server_port");
            entity.Property(e => e.DiscordChannelId).HasColumnName("discord_channel_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("match")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.GroupId, "group_id");

            entity.HasIndex(e => e.Player1BanSongId, "player1_ban_song_id");

            entity.HasIndex(e => e.Player1Id, "player1_id");

            entity.HasIndex(e => e.Player2BanSongId, "player2_ban_song_id");

            entity.HasIndex(e => e.Player2Id, "player2_id");

            entity.HasIndex(e => e.RefereeId, "referee_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.EndTimestamp)
                .HasColumnType("timestamp")
                .HasColumnName("end_timestamp");
            entity.Property(e => e.GroupId)
                .HasColumnType("int(11)")
                .HasColumnName("group_id");
            entity.Property(e => e.Player1BanSongId)
                .HasColumnType("int(11)")
                .HasColumnName("player1_ban_song_id");
            entity.Property(e => e.Player1Id)
                .HasColumnType("int(11)")
                .HasColumnName("player1_id");
            entity.Property(e => e.Player1WinCount)
                .HasColumnType("int(11)")
                .HasColumnName("player1_win_count");
            entity.Property(e => e.Player2BanSongId)
                .HasColumnType("int(11)")
                .HasColumnName("player2_ban_song_id");
            entity.Property(e => e.Player2Id)
                .HasColumnType("int(11)")
                .HasColumnName("player2_id");
            entity.Property(e => e.Player2WinCount)
                .HasColumnType("int(11)")
                .HasColumnName("player2_win_count");
            entity.Property(e => e.RefereeId)
                .HasColumnType("int(11)")
                .HasColumnName("referee_id");
            entity.Property(e => e.RefereeNotes)
                .HasColumnType("text")
                .HasColumnName("referee_notes");
            entity.Property(e => e.StartTimestamp)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("start_timestamp");
            entity.Property(e => e.WinningPlayerNum)
                .HasColumnType("int(11)")
                .HasColumnName("winning_player_num");

            entity.HasOne(d => d.Group).WithMany(p => p.Matches)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("match_ibfk_1");

            entity.HasOne(d => d.Player1BanSong).WithMany(p => p.MatchPlayer1BanSongs)
                .HasForeignKey(d => d.Player1BanSongId)
                .HasConstraintName("match_ibfk_5");

            entity.HasOne(d => d.Player1).WithMany(p => p.MatchPlayer1s)
                .HasForeignKey(d => d.Player1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("match_ibfk_2");

            entity.HasOne(d => d.Player2BanSong).WithMany(p => p.MatchPlayer2BanSongs)
                .HasForeignKey(d => d.Player2BanSongId)
                .HasConstraintName("match_ibfk_6");

            entity.HasOne(d => d.Player2).WithMany(p => p.MatchPlayer2s)
                .HasForeignKey(d => d.Player2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("match_ibfk_3");

            entity.HasOne(d => d.Referee).WithMany(p => p.MatchReferees)
                .HasForeignKey(d => d.RefereeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("match_ibfk_4");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("participant")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.DiscordId, "discord_id");

            entity.HasIndex(e => e.GroupId, "group_id");

            entity.HasIndex(e => e.Name, "name");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.AdvancedToPlayoff)
                .HasDefaultValueSql("'0'")
                .HasColumnName("advanced_to_playoff");
            entity.Property(e => e.DiscordId).HasColumnName("discord_id");
            entity.Property(e => e.GroupId)
                .HasColumnType("int(11)")
                .HasColumnName("group_id");
            entity.Property(e => e.GroupRank)
                .HasColumnType("int(11)")
                .HasColumnName("group_rank");
            entity.Property(e => e.IsCompetitor)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_competitor");
            entity.Property(e => e.IsReferee)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_referee");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Pronouns)
                .HasMaxLength(255)
                .HasColumnName("pronouns");
            entity.Property(e => e.QualifierRank)
                .HasColumnType("int(11)")
                .HasColumnName("qualifier_rank");
            entity.Property(e => e.QualifierScore)
                .HasColumnType("int(11)")
                .HasColumnName("qualifier_score");
            entity.Property(e => e.ReceivedGroupBye)
                .HasDefaultValueSql("'0'")
                .HasColumnName("received_group_bye");

            entity.HasOne(d => d.Group).WithMany(p => p.Participants)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("participant_ibfk_1");
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => new { e.MatchId, e.RoundNum, e.SeqNum })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity
                .ToTable("round")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.PickedSongId, "picked_song_id");

            entity.Property(e => e.MatchId)
                .HasColumnType("int(11)")
                .HasColumnName("match_id");
            entity.Property(e => e.RoundNum)
                .HasColumnType("int(11)")
                .HasColumnName("round_num");
            entity.Property(e => e.SeqNum)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("seq_num");
            entity.Property(e => e.ForefitPlayerNum)
                .HasColumnType("int(11)")
                .HasColumnName("forefit_player_num");
            entity.Property(e => e.PickedSongId)
                .HasColumnType("int(11)")
                .HasColumnName("picked_song_id");
            entity.Property(e => e.PickingPlayerNum)
                .HasColumnType("int(11)")
                .HasColumnName("picking_player_num");
            entity.Property(e => e.Player1Score)
                .HasColumnType("int(11)")
                .HasColumnName("player1_score");
            entity.Property(e => e.Player2Score)
                .HasColumnType("int(11)")
                .HasColumnName("player2_score");
            entity.Property(e => e.RefereeNotes)
                .HasColumnType("text")
                .HasColumnName("referee_notes");
            entity.Property(e => e.ResultsScreenshot)
                .HasColumnType("mediumblob")
                .HasColumnName("results_screenshot");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("timestamp")
                .HasColumnName("timestamp");
            entity.Property(e => e.WinningPlayerNum)
                .HasColumnType("int(11)")
                .HasColumnName("winning_player_num");

            entity.HasOne(d => d.Match).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("round_ibfk_1");

            entity.HasOne(d => d.PickedSong).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.PickedSongId)
                .HasConstraintName("round_ibfk_2");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("song")
                .HasCharSet("utf8mb3")
                .UseCollation("utf8mb3_general_ci");

            entity.HasIndex(e => e.FocusId, "focus_id");

            entity.HasIndex(e => e.Name, "name");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Artist)
                .HasMaxLength(255)
                .HasColumnName("artist");
            entity.Property(e => e.Charter)
                .HasMaxLength(255)
                .HasColumnName("charter");
            entity.Property(e => e.FocusId)
                .HasColumnType("int(11)")
                .HasColumnName("focus_id");
            entity.Property(e => e.Modifiers)
                .HasMaxLength(255)
                .HasColumnName("modifiers");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Source)
                .HasMaxLength(255)
                .HasColumnName("source");

            entity.HasOne(d => d.Focus).WithMany(p => p.Songs)
                .HasForeignKey(d => d.FocusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("song_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
