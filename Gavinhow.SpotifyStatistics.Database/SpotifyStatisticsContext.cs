using System;
using System.Linq;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Gavinhow.SpotifyStatistics.Database
{
    public class SpotifyStatisticsContext : DbContext
    {
        public SpotifyStatisticsContext(DbContextOptions<SpotifyStatisticsContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Play> Plays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema(schema: DBGlobals.SchemaName);
            base.OnModelCreating(modelBuilder);


        }

        public override int SaveChanges()
        {
            AddAuitInfo();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            AddAuitInfo();
            return await base.SaveChangesAsync();
        }

        private void AddAuitInfo()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((BaseEntity)entry.Entity).Created = DateTime.UtcNow;
                }
            ((BaseEntity)entry.Entity).Modified = DateTime.UtcNow;
            }
        }
    }
}
