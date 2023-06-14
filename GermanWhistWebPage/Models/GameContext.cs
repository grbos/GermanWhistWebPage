using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

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

            // The following configures EF to create a Sqlite database file in the
            // special "local" folder for your platform.

            modelBuilder.Entity<Game>()
                        .Property(e => e.CardStack)
                        .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                            v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                            new ValueComparer<ICollection<int>>(
                                (c1, c2) => c1.SequenceEqual(c2),
                                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                                c => c.ToList()));

            modelBuilder.Entity<Game>()
                .Property(b => b.HandPlayer1)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    new ValueComparer<List<int>>(
                        (c1, c2) => c1.OrderBy(i => i).SequenceEqual(c2.OrderBy(i => i)),
                        c => c.OrderBy(i => i).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.OrderBy(i => i).ToList()));

            modelBuilder.Entity<Game>()
                .Property(b => b.HandPlayer2)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    new ValueComparer<List<int>>(
                        (c1, c2) => c1.OrderBy(i => i).SequenceEqual(c2.OrderBy(i => i)),
                        c => c.OrderBy(i => i).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.OrderBy(i => i).ToList()));
        }
    }


}