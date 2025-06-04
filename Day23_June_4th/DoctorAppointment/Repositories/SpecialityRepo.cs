using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;

public class SpecialityRepo : Repository<int, Speciality>
{
    public SpecialityRepo(ClinicContext clinicContext) : base(clinicContext)
    {
            
    }

    public override async Task<Speciality> Get(int key)
    {
        var speciality = await _clinicContext.specialities.SingleOrDefaultAsync(d => d.Id == key);
        return speciality ?? throw new Exception("No Speciality with teh given ID");
        
    }

    public override async Task<IEnumerable<Speciality>> GetAll()
    {
         var specialities = _clinicContext.specialities;
            if (specialities.Count() == 0)
                throw new Exception("No Specialities in the database");
            return await specialities.ToListAsync();
    }
}