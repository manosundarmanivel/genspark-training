using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

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
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Convert List<string> Tags to JSON
                entity.Property(e => e.Tags)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                      );

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
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
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

            modelBuilder.Entity<CouponCode>(entity =>
{
    entity.HasIndex(c => c.Code).IsUnique();
    entity.Property(c => c.DiscountAmount).HasColumnType("decimal(10,2)");
    entity.Property(c => c.DiscountPercentage).HasColumnType("decimal(5,2)");
});

            modelBuilder.Entity<Transaction>()
                   .HasOne(t => t.User)
                   .WithMany() 
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Course)
                .WithMany() 
                .HasForeignKey(t => t.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}
