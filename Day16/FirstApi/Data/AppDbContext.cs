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

    }
}
