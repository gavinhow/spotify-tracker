using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.EntityFrameworkCore;

namespace Gavinhow.SpotifyStatistics.Database
{
    public class SpotifyStatisticsContext : DbContext
    {
        public SpotifyStatisticsContext(DbContextOptions<SpotifyStatisticsContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Play> Plays { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<ArtistAlbum> ArtistAlbums { get; set; }
        public DbSet<ArtistTrack> ArtistTracks { get; set; }
        public DbSet<ImportLog> ImportLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(schema: DBGlobals.SchemaName);

            modelBuilder.Entity<Play>().HasKey(c => new { c.TrackId, c.TimeOfPlay, c.UserId });
            modelBuilder.Entity<ArtistAlbum>().HasKey(c => new { c.AlbumId, c.ArtistId });
            modelBuilder.Entity<ArtistTrack>().HasKey(c => new { c.ArtistId, c.TrackId });
            modelBuilder.Entity<Friend>().HasKey(c => new { c.UserId, c.FriendId });
            modelBuilder.Entity<Play>()
                .Property<string>("TrackId");

            modelBuilder.Entity<Play>()
                .HasOne(p => p.Track)
                .WithMany()
                .HasForeignKey(p => p.TrackId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            modelBuilder.Entity<ImportLog>()
                .HasOne(il => il.User)
                .WithMany(u => u.ImportLogs)
                .HasForeignKey(il => il.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

    public static class DbSetExtensions
    {
        public static Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<T> AddIfNotExists<T>(
            this DbSet<T> dbSet, 
            T entity, 
            Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            var exists = predicate != null 
                ? dbSet.Local.Any(predicate.Compile()) || dbSet.Any(predicate)
                : dbSet.Local.Any() || dbSet.Any();
        
            return !exists ? dbSet.Add(entity) : null;
        }
    }
}