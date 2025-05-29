

using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

public class DoctorRepository : Repository<int, Doctor>
{
    public DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }

    public override async Task<Doctor> Get(int key)
    {
        var doctor = await _clinicContext.doctors.Include(d => d.DoctorSpecialities).ThenInclude(s => s.Speciality).SingleOrDefaultAsync(d => d.Id == key);
        return doctor ?? throw new Exception("No doctor with the given ID");
    }

    public override async Task<IEnumerable<Doctor>> GetAll()
    {
        var doctors = await _clinicContext.doctors.Include(d => d.DoctorSpecialities).ThenInclude(s =>s.Speciality).ToListAsync();
        if (doctors.Count() == 0)
            throw new Exception("No Doctors Found");
        return doctors;
    }

}