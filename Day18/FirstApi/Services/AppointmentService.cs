using FirstApi.Models;
using FirstApi.Repositories.Interfaces;
using FirstApi.Services.Interfaces;
using FirstApi.Models.DTOs;

namespace FirstApi.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _repo;

        public AppointmentService(IRepository<Appointment> repo)
        {
            _repo = repo;
        }

        public async Task<List<Appointment>> GetAllAsync() => await _repo.GetAllAsync();

        public async Task<Appointment?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task AddAsync(Appointment entity) => await _repo.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        public async Task<List<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            var allAppointments = await _repo.GetAllAsync();
            return allAppointments
                .Where(a => a.DoctorId == doctorId)
                .ToList();
        }

        public async Task<List<Appointment>> GetByDateAsync(DateTime date)
        {
            var allAppointments = await _repo.GetAllAsync();
            return allAppointments
                .Where(a => a.AppointmentDate.Date == date.Date)
                .ToList();
        }
    }
}
