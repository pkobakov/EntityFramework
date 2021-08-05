namespace VaporStore.Data
{
	using Microsoft.EntityFrameworkCore;
    using VaporStore.Data.Models;

    public class VaporStoreDbContext : DbContext
	{
		public VaporStoreDbContext()
		{
		}

		public VaporStoreDbContext(DbContextOptions options)
			: base(options)
		{
		}

        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<GameTag> GameTags { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			if (!options.IsConfigured)
			{
				options
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder model)
		{

			model.Entity<GameTag>(entity=> 
			{
				entity.HasKey(game => new { game.GameId, game.TagId });
				entity.HasOne(game => game.Game)
					  .WithMany(tag => tag.GameTags)
					  .HasForeignKey(game => game.GameId)
					  .OnDelete(DeleteBehavior.Restrict);

				entity.HasOne(tag=> tag.Tag)
				      .WithMany (game => game.GameTags)
					  .HasForeignKey(tag=>tag.TagId)
					  .OnDelete(DeleteBehavior.Restrict);


			});
		}
	}
}