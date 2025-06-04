using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;

namespace DoctorAppointment.Interfaces
{
    public interface IDoctorService
    {
        // public Task<Doctor> GetDoctByName(string name);
        public Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality);
        public Task<Doctor> AddDoctor(DoctorAddDto doctor);
        // public  Task<IEnumerable<DisplayDoctorDto>> GetDoctors();
    }
}