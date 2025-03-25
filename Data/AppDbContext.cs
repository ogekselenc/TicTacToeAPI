using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TicTacToeAPI.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Move> Moves { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    protected String Message = "Host=localhost;Port=5432;Database=tictactoe;Username=ognjen \n;Password=ekselencija84.";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(Message);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasIndex(e => e.PlayerId, "IX_Games_PlayerId");

            entity.HasIndex(e => e.PlayerOid, "IX_Games_PlayerOId");

            entity.HasIndex(e => e.PlayerXid, "IX_Games_PlayerXId");

            entity.Property(e => e.PlayerOid).HasColumnName("PlayerOId");
            entity.Property(e => e.PlayerXid).HasColumnName("PlayerXId");

            entity.HasOne(d => d.Player).WithMany(p => p.GamePlayers).HasForeignKey(d => d.PlayerId);

            entity.HasOne(d => d.PlayerO).WithMany(p => p.GamePlayerOs)
                .HasForeignKey(d => d.PlayerOid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.PlayerX).WithMany(p => p.GamePlayerXes)
                .HasForeignKey(d => d.PlayerXid)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasIndex(e => e.GameId, "IX_Moves_GameId");

            entity.HasIndex(e => e.PlayerId, "IX_Moves_PlayerId");

            entity.HasOne(d => d.Game).WithMany(p => p.Moves)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Player).WithMany(p => p.Moves)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
