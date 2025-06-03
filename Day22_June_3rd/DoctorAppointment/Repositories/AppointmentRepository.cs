using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Repositories;
using Microsoft.EntityFrameworkCore;

public class AppointmentRepo : Repository<int, Appointment>
{
    public AppointmentRepo(ClinicContext clinicContext) : base(clinicContext)
    {

    }

    public async Task<Appointment> Add(Appointment appointment)
    {
        _clinicContext.appointments.Add(appointment);
        await _clinicContext.SaveChangesAsync();
        return appointment;
    }

 public async Task<Appointment> Update(Appointment appointment)
{
    _clinicContext.appointments.Update(appointment);
    await _clinicContext.SaveChangesAsync();
    return appointment;
}


    public override async Task<Appointment> Get(int key)
    {
        var appointment = await _clinicContext.appointments.SingleOrDefaultAsync(d => d.Id == key);
        return appointment ?? throw new Exception("No Appointment with the given ID");

    }

    public override async Task<IEnumerable<Appointment>> GetAll()
    {
        var appointments = _clinicContext.appointments;
        if (appointments.Count() == 0)
            throw new Exception("No Appointments in the database");
        return await appointments.ToListAsync();
    }
    


}