using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Data
{
    public class TicTacToeDbContext : DbContext
    {
        public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerX)
                .WithMany()
                .HasForeignKey(g => g.PlayerXId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.PlayerO)
                .WithMany()
                .HasForeignKey(g => g.PlayerOId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Move>()
                .HasOne(m => m.Game)
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Move>()
                .HasOne(m => m.Player)
                .WithMany()
                .HasForeignKey(m => m.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
