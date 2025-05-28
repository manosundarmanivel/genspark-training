using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;
using FirstApi.Models.DTOs;

namespace FirstApi.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _repo;

        public PatientService(IRepository<Patient> repo)
        {
            _repo = repo;
        }

        public async Task<List<PatientDTO>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();
            return patients.Select(p => new PatientDTO
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Gender = p.Gender,
                Address = p.Address
            }).ToList();
        }

        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient == null) return null;

            return new PatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Age = patient.Age,
                Gender = patient.Gender,
                Address = patient.Address
            };
        }

        public async Task AddAsync(PatientDTO dto)
        {
            var patient = new Patient
            {
                Name = dto.Name,
                Age = dto.Age,
                Gender = dto.Gender,
                Address = dto.Address
            };
            await _repo.AddAsync(patient);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(int patientId)
        {
            var patient = await _repo.GetByIdAsync(patientId);
            return patient?.Appointments?.ToList() ?? new List<Appointment>();
        }
    }
}
