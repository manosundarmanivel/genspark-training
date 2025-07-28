using Microsoft.EntityFrameworkCore;
using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ContactU> ContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USER
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(100);
            });

            // CATEGORY
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });

            // COLOR
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(c => c.ColorId);
                entity.Property(c => c.Color1).IsRequired().HasMaxLength(50);
            });

            // MODEL
            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasKey(m => m.ModelId);
                entity.Property(m => m.Model1).IsRequired().HasMaxLength(100);
            });

            // PRODUCT
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Image).HasMaxLength(255);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Products)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Color)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.ColorId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Model)
                      .WithMany(m => m.Products)
                      .HasForeignKey(p => p.ModelId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // NEWS
            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(n => n.NewsId);
                entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
                entity.Property(n => n.ShortDescription).HasMaxLength(500);
                entity.Property(n => n.Image).HasMaxLength(255);
                entity.Property(n => n.Content).IsRequired();

                entity.HasOne(n => n.User)
                      .WithMany(u => u.News)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ORDER
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderID);
                entity.Property(o => o.OrderName).IsRequired().HasMaxLength(100);
                entity.Property(o => o.PaymentType).HasMaxLength(50);
                entity.Property(o => o.Status).HasMaxLength(50);
                entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(100);
                entity.Property(o => o.CustomerPhone).HasMaxLength(20);
                entity.Property(o => o.CustomerEmail).HasMaxLength(100);
                entity.Property(o => o.CustomerAddress).HasMaxLength(255);
            });

            // ORDER DETAIL (Composite Key)
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => new { od.OrderID, od.ProductID });

                entity.Property(od => od.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                      .WithMany(p => p.OrderDetails)
                      .HasForeignKey(od => od.ProductID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CONTACT US
            modelBuilder.Entity<ContactU>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Content).IsRequired().HasMaxLength(1000);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
