using FirstApi.Models;
using FirstApi.Static;
using FirstApi.Repositories.Interfaces;

namespace FirstApi.Repositories
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        public List<Appointment> GetAll() => DataStore.Appointments;
        public Appointment? GetById(int id) => DataStore.Appointments.FirstOrDefault(a => a.Id == id);
        public void Add(Appointment appointment)
        {
            appointment.Id = DataStore.Appointments.Count + 1;
            DataStore.Appointments.Add(appointment);
        }
        public void Delete(int id)
        {
            var appointment = GetById(id);
            if (appointment != null) DataStore.Appointments.Remove(appointment);
        }
    }
}