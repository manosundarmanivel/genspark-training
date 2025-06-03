
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;
namespace DoctorAppointment.Interfaces
{

    public interface IPatientService
    {
        public Task<Patient> Get(int key);
        public Task<IEnumerable<Patient>> GetAll();
        public Task<Patient> Add(PatientAddDto patient);
        public Task<Patient> Update(int key, Patient item);
        public Task<Patient> Delete(int key);
    }
}