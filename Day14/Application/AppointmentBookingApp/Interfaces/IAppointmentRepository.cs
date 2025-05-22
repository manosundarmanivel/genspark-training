using WholeApplication.Models;

namespace WholeApplication.Interfaces
{
    public interface IAppointmentRepository
    {
        void Save(Appointment appointment);
        List<Appointment> GetAppointments(DateTime date);
        void Remove(Appointment appointment);
    }
}