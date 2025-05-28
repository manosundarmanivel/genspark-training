using FirstApi.Models;
using FirstApi.Services.Interfaces;

public interface IAppointmentService : IService<Appointment>
{
    Task<List<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<List<Appointment>> GetByDateAsync(DateTime date);
}
