using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Linq;

namespace BookStorageApp.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
          
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.ChaptersOfBook)
                .WithOne(ch => ch.Book)
                .OnDelete(DeleteBehavior.Cascade);          
        }
    }
}
