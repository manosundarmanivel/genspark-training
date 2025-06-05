using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

public class DoctorRepo : Repository<int, Doctor>
{
    public DoctorRepo(ClinicContext clinicContext) : base(clinicContext)
    {
            
    }

    public override async Task<Doctor> Get(int key)
    {
        var doctor = await _clinicContext.doctors.SingleOrDefaultAsync(d => d.Id == key);
        return doctor ?? throw new Exception("No Doctor with the given ID");
        
    }

    public override async Task<IEnumerable<Doctor>> GetAll()
    {
         var doctors = await _clinicContext.doctors
        .Include(d => d.DoctorSpecialities)
        .ThenInclude(ds => ds.Speciality)
        .ToListAsync();
        if (doctors.Count() == 0)
                throw new Exception("No Doctors in the database");
        return  doctors.ToList();
    }
}