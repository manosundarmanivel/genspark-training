
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

namespace SecondWebApi.Repositories;

public class PatientRepository : Repository<int, Patient>
{
    public PatientRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }
    public override async Task<Patient> Get(int key)
    {
        var patient = await _clinicContext.patients.SingleOrDefaultAsync(p => p.Id == key);
        return patient ?? throw new Exception("No patient with the given ID");
    }

    public override async Task<IEnumerable<Patient>> GetAll()
    {
        var patients = await _clinicContext.patients.ToListAsync();
        if (patients.Count() == 0)
            throw new Exception("No Patients Found");
        return patients;
    }
}