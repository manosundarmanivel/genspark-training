using WholeApplication.Interfaces;
using WholeApplication.Models;

namespace WholeApplication.Repository
{
    public class InMemoryAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = new();

        public void Save(Appointment appointment)
        {
            _appointments.Add(appointment);
        }

        public List<Appointment> GetAppointments(DateTime date)
        {
            return _appointments.Where(a => a.Date.Date == date.Date).ToList();
        }

        public void Remove(Appointment appointment)
        {
            _appointments.Remove(appointment);
        }
    }
}
