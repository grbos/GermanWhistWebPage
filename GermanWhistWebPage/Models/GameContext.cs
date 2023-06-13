using Microsoft.EntityFrameworkCore;

namespace GermanWhistWebPage.Models
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
    : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Game>()
                .Property(b => b.CardStack)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            modelBuilder.Entity<Game>()
                .Property(b => b.HandPlayer1)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());

            modelBuilder.Entity<Game>()
                .Property(b => b.HandPlayer2)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
        }
    }

    
}