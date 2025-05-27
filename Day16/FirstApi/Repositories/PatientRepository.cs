using FirstApi.Models;
using FirstApi.Static;
using FirstApi.Repositories.Interfaces;

namespace FirstApi.Repositories
{
    public class PatientRepository : IRepository<Patient>
    {
        public List<Patient> GetAll() => DataStore.Patients;
        public Patient? GetById(int id) => DataStore.Patients.FirstOrDefault(p => p.Id == id);
        public void Add(Patient patient)
        {
            patient.Id = DataStore.Patients.Count + 1;
            DataStore.Patients.Add(patient);
        }
        public void Delete(int id)
        {
            var patient = GetById(id);
            if (patient != null) DataStore.Patients.Remove(patient);
        }
    }
}