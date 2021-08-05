namespace BookShop.Data
{
    using BookShop.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class BookShopContext : DbContext
    {
        public BookShopContext() { }

        public BookShopContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<AuthorBook> AuthorsBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AuthorBook>(authorBook =>
            {
                authorBook.HasKey(ab => new { ab.AuthorId, ab.BookId });
                authorBook.HasOne(a => a.Author)
                          .WithMany(ab => ab.AuthorsBooks)
                          .HasForeignKey(a => a.AuthorId)
                          .OnDelete(DeleteBehavior.Restrict);

                authorBook.HasOne(b => b.Book)
                          .WithMany(ba => ba.AuthorsBooks)
                          .HasForeignKey(b => b.BookId)
                          .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}