using Microsoft.EntityFrameworkCore;
using KisanStore.API.Models;

namespace KisanStore.Data
{
    public class KisanStoreDbContext : DbContext
    {
        public KisanStoreDbContext(DbContextOptions<KisanStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Configure Product Entity =====
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.DiscountPrice)
                    .HasPrecision(18, 2);

                // Configure relationship with Category
                entity.HasOne(p => p.Category)
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Configure Order Entity =====
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.TotalAmount)
                    .HasPrecision(18, 2);

                // Configure relationship with User
                entity.HasOne(o => o.User)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure relationship with OrderItems
                entity.HasMany(o => o.OrderItems)
                    .WithOne()
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Configure OrderItem Entity =====
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.TotalPrice)
                    .HasPrecision(18, 2);

                // Configure relationship with Product
                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Configure Cart Entity =====
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId);

                // Configure relationship with Product
                entity.HasOne(c => c.Product)
                    .WithMany()
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Configure Review Entity =====
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId);

                // Configure relationship with User
                entity.HasOne(r => r.User)
                    .WithMany()
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Configure User Entity =====
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });

            // ===== Configure Category Entity =====
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
            });
        }
    }
}