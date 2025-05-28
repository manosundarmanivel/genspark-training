using Microsoft.EntityFrameworkCore;
using FirstApi.Models;

namespace FirstApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Speciality> Specialities { get; set; }
        public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key for DoctorSpeciality (many-to-many)
            modelBuilder.Entity<DoctorSpeciality>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecialityId });

            // DoctorSpeciality relationship
            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecialities)
                .HasForeignKey(ds => ds.DoctorId);

            modelBuilder.Entity<DoctorSpeciality>()
                .HasOne(ds => ds.Speciality)
                .WithMany(s => s.DoctorSpecialities)
                .HasForeignKey(ds => ds.SpecialityId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
