using Chess.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chess.DataAccess
{
    public class DataContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<Side> Sides { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

        }
    }
}
