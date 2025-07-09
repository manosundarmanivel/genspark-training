using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ElearnAPI.Data
{
    public class ElearnDbContext : DbContext
    {
        public ElearnDbContext(DbContextOptions<ElearnDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<UserFileProgress> UserFileProgresses { get; set; }
        public DbSet<CouponCode> CouponCodes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Role
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Instructor" },
                new Role { Id = 3, Name = "Student" }
            );

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime2");

                entity.HasOne(e => e.Role)
                      .WithMany()
                      .HasForeignKey("RoleId")
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Domain).IsRequired();
                entity.Property(e => e.Level).HasMaxLength(50);
                entity.Property(e => e.Language).HasMaxLength(50);
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Price).HasPrecision(18, 2);

                // Convert List<string> Tags to JSON
                var converter = new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

                var comparer = new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                );

                entity.Property(e => e.Tags)
                      .HasConversion(converter)
                      .Metadata.SetValueComparer(comparer);

                entity.HasOne(e => e.Instructor)
                      .WithMany()
                      .HasForeignKey(e => e.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.UploadedFiles)
                      .WithOne(f => f.Course!)
                      .HasForeignKey(f => f.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UploadedFile
            modelBuilder.Entity<UploadedFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FileType).HasMaxLength(10);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Path).IsRequired();
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.UserId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFileProgress
            modelBuilder.Entity<UserFileProgress>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsCompleted).HasDefaultValue(false);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.FileProgresses)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.UploadedFile)
                      .WithMany(f => f.FileProgresses)
                      .HasForeignKey(e => e.UploadedFileId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Coupon
            modelBuilder.Entity<CouponCode>(entity =>
            {
                entity.HasIndex(c => c.Code).IsUnique();
                entity.Property(c => c.DiscountAmount).HasPrecision(10, 2);
                entity.Property(c => c.DiscountPercentage).HasPrecision(5, 2);
            });

            // Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(t => t.Amount).HasPrecision(18, 2);

                entity.HasOne(t => t.User)
                      .WithMany()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Course)
                      .WithMany()
                      .HasForeignKey(t => t.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
