using DoctorAppointment.Contexts;
using DoctorAppointment.Interfaces;
using DoctorAppointment.Models;
using DoctorAppointment.Models.DTO;

namespace DoctorAppointment.Service
{
    class SpecialityService : ISpecialityServices
    {

        private SpecialityRepo _repo;
        public SpecialityService(SpecialityRepo repo)
        {
            _repo = repo;
        }
        public async Task<Speciality> AddSpeciality(SpecialityAddDto speciality)
        {
            var _speciality = new Speciality
            {
                Name = speciality.Name,
            };

            return await _repo.Add(_speciality);

        }

        public async Task<IEnumerable<Speciality>> GetSpecialities()
        {
            return await _repo.GetAll();
        }
    }
}