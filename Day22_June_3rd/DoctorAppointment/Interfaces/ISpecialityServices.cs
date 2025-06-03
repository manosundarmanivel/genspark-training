using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;

namespace DoctorAppointment.Interfaces
{
    public interface ISpecialityServices
    {
        public Task<Speciality> AddSpeciality(SpecialityAddDto speciality);
        public Task<IEnumerable<Speciality>> GetSpecialities();
    }
}