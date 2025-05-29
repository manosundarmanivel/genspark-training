
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

public class SpecialityRepository : Repository<int, Speciality>
{
    public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }
    public override async Task<Speciality> Get(int key)
    {
        var speciality = await _clinicContext.specialities.Include(ds => ds.DoctorSpecialities).SingleOrDefaultAsync(d => d.Id == key);
        return speciality ?? throw new Exception("No speciality with the given ID");
    }

    public override async Task<IEnumerable<Speciality>> GetAll()
    {
        var specialities = await _clinicContext.specialities.Include(ds => ds.DoctorSpecialities).ToListAsync();
        if (specialities.Count() == 0)
            throw new Exception("No Specialities Found");
        return specialities;
        
    }
}