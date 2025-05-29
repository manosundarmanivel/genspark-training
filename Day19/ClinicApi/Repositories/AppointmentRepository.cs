
using Microsoft.EntityFrameworkCore;
using SecondWebApi.Contexts;
using SecondWebApi.Models;
using SecondWebApi.Repositories;

public class AppointmentRepository : Repository<string, Appointment>
{
    public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
    {

    }

    public override async Task<Appointment> Get(string key)
    {
        var appointments = await _clinicContext.appointmnets.SingleOrDefaultAsync(d => d.AppointmnetNumber == key);
        return appointments ?? throw new Exception("No patient with the given ID");
    }

    public override async Task<IEnumerable<Appointment>> GetAll()
    {
        var appointments = await _clinicContext.appointmnets.ToListAsync();
        if (appointments.Count() == 0)
            throw new Exception("No Doctors Found");
        return appointments;
    }
}