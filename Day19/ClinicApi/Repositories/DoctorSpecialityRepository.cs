
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

public class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
{
    public DoctorSpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }
    public override async Task<DoctorSpeciality> Get(int key)
    {
        var doctorspeciality = await _clinicContext.doctorSpecialities.SingleOrDefaultAsync(d => d.SerialNumber == key);
        return doctorspeciality ?? throw new Exception("No speciality with the given ID");
    }

    public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
    {
        var doctorspecialities = await _clinicContext.doctorSpecialities.ToListAsync();
        if (doctorspecialities.Count() == 0)
            throw new Exception("No Specialities Found");
        return doctorspecialities;
    }
}