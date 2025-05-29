using Microsoft.EntityFrameworkCore;
using SecondWebApi.Models;
namespace SecondWebApi.Contexts;

public class ClinicContext : DbContext
{
    public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
    {

    }

    public DbSet<Patient> patients { get; set; }
    public DbSet<Speciality> specialities { get; set; }
    public DbSet<Appointment> appointmnets { get; set; }
    public DbSet<Doctor> doctors { get; set; }
    public DbSet<DoctorSpeciality> doctorSpecialities { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmnetNumber).HasName("PK_Appointment_Number");

        modelBuilder.Entity<Appointment>().HasOne(app => app.Patient)
                                            .WithMany(p => p.Appointmnets)
                                            .HasForeignKey(ap => ap.PatientId)
                                            .HasConstraintName("FK_Appoinment_Patient")
                                            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Appointment>().HasOne(app => app.Doctor)
                                            .WithMany(d => d.Appointmnets)
                                            .HasForeignKey(ad => ad.DoctorId)
                                            .HasConstraintName("FK_Appoinment_Doctor")
                                            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DoctorSpeciality>().HasKey(ds => ds.SerialNumber);

        modelBuilder.Entity<DoctorSpeciality>().HasOne(sp => sp.Speciality)
                                                .WithMany(s => s.DoctorSpecialities)
                                                .HasForeignKey(ds => ds.SpecialityId)
                                                .HasConstraintName("FK_Speciality_Spec")
                                                .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DoctorSpeciality>().HasOne(sp => sp.Doctor)
                                        .WithMany(s => s.DoctorSpecialities)
                                        .HasForeignKey(ds => ds.DoctorId)
                                        .HasConstraintName("FK_Speciality_Doctor")
                                        .OnDelete(DeleteBehavior.Restrict);

    }
    
    public async Task<List<Doctor>> GetDoctorsBySpecialityFromSP(string specialityName)
        {
        return await doctors
            .FromSqlInterpolated($"SELECT * FROM proc_GetDoctorsBySpeciality({specialityName})")
    .ToListAsync();
        }

}

