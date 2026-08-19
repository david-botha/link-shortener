using LinkShortener.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Link> Links => Set<Link>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>().HasIndex(link => link.Slug).IsUnique();
        }
    }
}
