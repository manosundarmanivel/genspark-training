using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.InstructorId).IsRequired();
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict);

       
                entity.HasMany(e => e.UploadedFiles)
                      .WithOne(f => f.Course!)
                      .HasForeignKey(f => f.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UploadedFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FileType).HasMaxLength(10);
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CourseId).IsRequired();

           
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Instructor" },
                new Role { Id = 3, Name = "Student" }
            );

            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.UserId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
