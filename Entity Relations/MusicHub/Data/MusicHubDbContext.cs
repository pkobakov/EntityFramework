namespace MusicHub.Data
{
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Song>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(20);
                entity.Property(s => s.Duration).IsRequired();
                entity.Property(s => s.CreatedOn).IsRequired();
                entity.HasOne(s => s.Album).WithMany(a => a.Songs).HasForeignKey(s=>s.AlbumId);
                entity.HasOne(s => s.Writer).WithMany(w => w.Songs).HasForeignKey(s => s.WriterId);
            });

            builder.Entity<Album>(entity=> 
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(40);
                entity.Property(a => a.ReleaseDate).IsRequired();
                entity.HasOne(a => a.Producer).WithMany(p => p.Albums).HasForeignKey(a=>a.ProducerId);
            });


            builder.Entity<Performer>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(20);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(20);
                entity.Property(p => p.Age).IsRequired();
                entity.Property(p => p.NetWorth).IsRequired();
            });

            builder.Entity<Producer>(entity =>
            {
                entity.HasKey(prod => prod.Id);
                entity.Property(prod => prod.Name).IsRequired().HasMaxLength(20);
               
            });

            builder.Entity<Writer>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.Name).IsRequired().HasMaxLength(20);

            });

            builder.Entity<SongPerformer>(entity=> 
            {
                entity.HasKey(sp => new { sp.SongId, sp.PerformerId});
                entity.HasOne(s => s.Performer).WithMany(p => p.PerformerSongs).HasForeignKey(s => s.PerformerId);
                entity.HasOne(p => p.Song).WithMany(s => s.SongPerformers).HasForeignKey(p => p.SongId);
            
            
            });


        }
    }
}
