
using FirstApi.Models;
using FirstApi.Static;
using FirstApi.Repositories.Interfaces;

namespace FirstApi.Repositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        public List<Doctor> GetAll() => DataStore.Doctors;
        public Doctor? GetById(int id) => DataStore.Doctors.FirstOrDefault(d => d.Id == id);
        public void Add(Doctor doctor)
        {
            doctor.Id = DataStore.Doctors.Count + 1;
            DataStore.Doctors.Add(doctor);
        }
        public void Delete(int id)
        {
            var doctor = GetById(id);
            if (doctor != null) DataStore.Doctors.Remove(doctor);
        }
    }
}
