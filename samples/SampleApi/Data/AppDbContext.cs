using Microsoft.EntityFrameworkCore;
using SampleApi.Models;

namespace SampleApi.Data
{
    /// <summary>
    /// Application database context
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Products table
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.DeletedAt);
                
                // Create index on UpdatedAt for efficient querying
                entity.HasIndex(e => e.UpdatedAt);
                entity.HasIndex(e => e.DeletedAt);
            });
        }
    }
}


