using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;

public class DoctorSpecialityRepo : Repository<int, DoctorSpeciality>
{
    public DoctorSpecialityRepo(ClinicContext clinicContext) : base(clinicContext)
    {
            
    }
     public override Task<DoctorSpeciality> Get(int key)
    {
        throw new NotSupportedException("Use Get(doctorId, specialityId) instead.");
    }
    public  async Task<DoctorSpeciality> Get(int doctorId, int specialityId)
    {
    var result = await _clinicContext.doctorSpecialities
        .SingleOrDefaultAsync(ds => ds.DoctorId == doctorId && ds.SpecialityId == specialityId);

    return result ?? throw new Exception("DoctorSpeciality not found");
    }

    public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
    {
         var doctorSpecialities = _clinicContext.doctorSpecialities;
            if (doctorSpecialities.Count() == 0)
                throw new Exception("No DoctorSpecialitys in the database");
            return await doctorSpecialities.ToListAsync();
    }
}